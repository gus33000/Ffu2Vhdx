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

## Usage

### Command

```batch
Ffu2Vhdx E:\DeviceImage\Flash.ffu E:\DeviceImage\FlashOutput
```

### Command Output

```batch
FFU Image To VHDx(s) tool
Version: 1.0.0.0

Store: 0
Size: 4194304
Device Path: VenHw(8D90D477-39A3-4A38-AB9E-586FF69ED051): UFS (LUN 1)
Destination: E:\DeviceImage\FlashOutput\Store0_VenHw(8D90D477-39A3-4A38-AB9E-586FF69ED051).vhdx
Dumping Store 0
[===========================100%===========================] 128MB/s 00:00:00.0

Store: 1
Size: 4194304
Device Path: VenHw(EDF85868-87EC-4F77-9CDA-5F10DF2FE601): UFS (LUN 2)
Destination: E:\DeviceImage\FlashOutput\Store1_VenHw(EDF85868-87EC-4F77-9CDA-5F10DF2FE601).vhdx
Dumping Store 1
[===========================100%===========================] 156MB/s 00:00:00.0

Store: 2
Size: 4294967296
Device Path: VenHw(D33F1985-F107-4A85-BE38-68DC7AD32CEA): UFS (LUN 4)
Destination: E:\DeviceImage\FlashOutput\Store2_VenHw(D33F1985-F107-4A85-BE38-68DC7AD32CEA).vhdx
Dumping Store 2
[===========================100%===========================] 233MB/s 00:00:00.0

Store: 3
Size: 28622979072
Device Path: VenHw(860845C1-BE09-4355-8BC1-30D64FF8E63A): UFS (LUN 0)
Destination: E:\DeviceImage\FlashOutput\Store3_VenHw(860845C1-BE09-4355-8BC1-30D64FF8E63A).vhdx
Dumping Store 3
[===========================100%===========================] 105MB/s 00:00:00.0

The operation completed successfully.
```

### Result

```batch
E:.
└───DeviceImage
    |   Flash.ffu
    └───FlashOutput
            Store0_VenHw(8D90D477-39A3-4A38-AB9E-586FF69ED051).vhdx
            Store1_VenHw(EDF85868-87EC-4F77-9CDA-5F10DF2FE601).vhdx
            Store2_VenHw(D33F1985-F107-4A85-BE38-68DC7AD32CEA).vhdx
            Store3_VenHw(860845C1-BE09-4355-8BC1-30D64FF8E63A).vhdx
```

## Demo

https://github.com/user-attachments/assets/922ec96a-1ed4-499b-b3e1-1eb8c11a5c7e
