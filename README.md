<div align="center">

# BP.ColourChimp

.NET 4.6.1 application written in WPF to provide a tool to help compare and convert colours. Supports ARGB, CMYK and HSV colour spaces.

[![GitHub release](https://img.shields.io/github/release/benpollarduk/colour-chimp.svg)](https://github.com/benpollarduk/colour-chimp/releases)
[![License](https://img.shields.io/github/license/benpollarduk/colour-chimp.svg)](https://opensource.org/licenses/MIT)

</div>

## Introduction 
![image](https://user-images.githubusercontent.com/129943363/230964979-3f6590a9-3b8e-4241-b57e-0b8480750f02.png)

Provides a simple information window for any of the colours that can be used to convert formats:

![image](https://user-images.githubusercontent.com/129943363/230965244-60db7d3c-c951-429e-829c-370f47d06928.png)

Provides functionality to gather colours from open windows and screen regions, and supports importing of colours from an image. Colours can be filtered and sorted.

## Notes
This is an old project now, it was started in 2010 and has been dormant for many years although the tool itself has proven to be useful numerous times. It has received some quality of life updates, but the underlying architecture is fundamentally flawed and requires revision before being developed any further. Colours are stored as rectangles in a UICollection which makes overall management slow and cumbersome. Colours should be broken away from the UI itself.

## Prerequisites
 * Windows
   * Download free IDE Visual Studio 2022 Community ( >> https://visualstudio.microsoft.com/de/vs/community/ ), or use commercial Visual Studio 2022 Version.

## Getting Started
 * Clone the repo.
 * Build all projects.
 * Run the BP.ColourChimp project.

## For Open Questions
Visit https://github.com/benpollarduk/colour-chimp/issues
