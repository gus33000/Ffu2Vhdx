# Ffu2Vhdx - Converts any FFU into VHDX file(s)

## Copyright

Copyright (c) 2024, Gustave Monce - gus33000.me - @gus33000

This software is released under the MIT license, for more information please see LICENSE.md

## Description

This tool enables converting Full Flash Update files (FFUs) into VHDX file(s).

It notably supports every known FFU file format to date, that is:

- Version 1
- Version 1.1 (With added compression support)
- Version 2 (With added multi store support)

For Version 1 FFUs, the tool will create only one VHDX file.

For Version 1.1 FFUs, conversion is only possible currently on Windows hosts and the tool will create only one VHDX file.

For Version 2 FFUs, the tool will create more than one VHDX File.