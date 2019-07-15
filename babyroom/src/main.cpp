#include "Arduino.h"

#include <memory>
#include <string>

#include <pgmspace.h>

#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>
#include <DHT.h>
#include <DHT_U.h>
#include <Wire.h>
#include <ACROBOTIC_SSD1306.h>
#include <RtcDS3231.h>

#include "util.h"

namespace
{

typedef RtcDS3231<TwoWire> RtcType;

enum
{
  kDefaultSensPin = D4,
  kDefaultSensType = DHT21
};

enum
{
  kDelayBetweenSamplesMillis = 2000
};

enum
{
  kBaudRate = 9600
};

enum
{
  kOnBoardLed = 2
};

static RtcType *RTC_Instance()
{
  static RtcType rtc(Wire);
  return &rtc;
}

static DHT_Unified *DHT_Instance()
{
  static DHT_Unified dht(kDefaultSensPin, kDefaultSensType);
  return &dht;
}

struct Configuration
{
  static Configuration *Instance();

  std::string ssid;
  bool is_use_password; // FIXME: use this
  std::string password;
  ESP8266WiFiMulti wifi;

protected:
  Configuration()
      : ssid("xxx"), // FIXME: set this via serial, store to flash
        is_use_password(true)
  {
    delay(10);
    wifi.addAP(ssid.c_str(), "xxx");
  }

private:
  DISALLOW_COPY_AND_ASSIGN(Configuration);
};

Configuration *Configuration::Instance()
{
  static Configuration config;
  return &config;
}

#define COUNT_OF(a) (sizeof(a) / sizeof(a[0]))

size_t __attribute__((nonnull(4)))
ReadingToString(float h, float t, const RtcDateTime &dt, String *out)
{
  char epochstr[20];
  StaticJsonDocument<JSON_OBJECT_SIZE(3)> jdoc;
  jdoc["humidity"] = h;
  jdoc["temperature"] = t;
  sprintf(epochstr, "%llu", dt.Epoch64Time());
  jdoc["timestamp"] = epochstr;
  return serializeJson(jdoc, *out);
}

bool SendUpdatedReading(float h, float t, const RtcDateTime &dt)
{
  String s;
  int rc = -1;
  size_t s_siz = ReadingToString(h, t, dt, &s);
  SerialLog(DEBUG, "serialized_size", s_siz);
  WiFiClient client;
  HTTPClient http;
  // FIXME: discover this via avahi/mDNS
  if (http.begin(client, "http://192.168.1.28:5000/sensors/add_reading"))
  {
    //SeriaLog(INFO, "payload", s);
    http.addHeader("Content-Type", "application/json");
    if ((rc = http.POST(s)) != 200)
    {
      SerialLog(INFO, "http_code", rc, "payload", s);
    }
    http.end();
  }
  return (rc == 200);
}

void DisplayReading(float h, float t, const RtcDateTime &dt)
{
  oled.setTextXY(0, 0);
  oled.putString("Humidity:");
  oled.setTextXY(2, 0);
  oled.putFloat(h, 2);
  oled.setTextXY(4, 0);
  oled.putString("Temperature:");
  oled.setTextXY(6, 0);
  oled.putFloat(t, 2);
  oled.setTextXY(7, 7);
  oled.putNumber(dt.Hour());
  oled.setTextXY(7, 10);
  oled.putNumber(dt.Minute());
  oled.setTextXY(7, 13);
  oled.putNumber(dt.Second());
}

void LogTime(const RtcDateTime &dt)
{
  char datestring[24];

  snprintf_P(datestring,
             COUNT_OF(datestring),
             PSTR("%02u/%02u/%04u %02u:%02u:%02u "),
             dt.Month(),
             dt.Day(),
             dt.Year(),
             dt.Hour(),
             dt.Minute(),
             dt.Second());
  Serial.print(datestring);
}

void SetupRtc(RtcType *rtc)
{
  rtc->Begin();
  RtcDateTime compiled = RtcDateTime(__DATE__, __TIME__);
  LogTime(compiled);
  SerialLog(INFO, "-- COMPILATION TIME --");
  if (rtc->IsDateTimeValid())
  {
    SerialLog(INFO, "RTC lost confidence in DateTime!");
    rtc->SetDateTime(compiled);
  }
  if (rtc->GetIsRunning())
  {
    SerialLog(INFO, "RTC was not running, starting now");
    rtc->SetIsRunning(true);
  }
  RtcDateTime now = RTC_Instance()->GetDateTime();
  if (now < compiled)
  {
    LogTime(now);
    SerialLog(INFO, "RTC older than compile time. Updating DateTime.");
    rtc->SetDateTime(compiled);
  }

  rtc->Enable32kHzPin(false);
  rtc->SetSquareWavePin(DS3231SquareWavePin_ModeNone);
}

} // anonymous namespace

void setup()
{
  Serial.begin(kBaudRate);
  Serial.println();
  SetupRtc(RTC_Instance());
  DHT_Instance()->begin();
  oled.init();
  oled.clearDisplay();
}

void loop()
{
  float h; // <! humidity
  float t; // <! temperature

  delay(kDelayBetweenSamplesMillis);

  if (!RTC_Instance()->IsDateTimeValid())
  {
    SerialLog(INFO, "RTC Lost Confidence in DateTime!");
  }
  sensors_event_t evt;
  DHT_Instance()->humidity().getEvent(&evt);
  h = evt.relative_humidity;
  DHT_Instance()->temperature().getEvent(&evt);
  t = evt.temperature;
  if (isnan(h) || isnan(t))
  {
    SerialLog(INFO, "Invalid reading!");
  }
  else
  {
    RtcDateTime now = RTC_Instance()->GetDateTime();
    LogTime(now);
    SerialLog(INFO, "humidity", h, "temperature", t);
    DisplayReading(h, t, now);
    if (Configuration::Instance()->wifi.run() == WL_CONNECTED)
    {
      SerialLog(INFO, "wifi_local_ip", WiFi.localIP());
      SendUpdatedReading(h, t, now);
    }
  }
}
