
// Placeholder for a better logging library.

#pragma once

#include "Arduino.h"

#define DISALLOW_COPY_AND_ASSIGN(TypeName) \
  TypeName(const TypeName &) = delete;     \
  void operator=(const TypeName &) = delete

enum LogLevel
{
  INFO = 0,
  DEBUG = 1
};

template <typename... Types>
void SerialLog(LogLevel level, Types... rest);

template <typename T>
void SeriaLog(LogLevel level, T t)
{
  Serial.print(t);
}

template <>
void SerialLog(LogLevel level)
{
  Serial.print(level == INFO ? " INFO" : " DEBUG");
  Serial.println();
}

template <typename T1, typename... Types>
void SerialLog(LogLevel level, T1 t, Types... rest)
{
  Serial.print(t);
  Serial.print(" ");
  SerialLog(level, rest...);
}
