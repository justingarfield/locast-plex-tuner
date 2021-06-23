# LocastPlexTuner

## Overview

This is a full port of the https://github.com/tgorgdotcom/locast2plex Python library.

## Under heavy development

This repository is currently a temporary name and the codeset is being heavily revised, refactored, and updated to clean-up anything that's out-dated at this point. Feel free to Clone / Fork, but just be aware that things are going to keep drastically changing until a few more bugs are ironed out.

### Known Issues

* Sports currently get considered as Movies. May need to create a new Plex Agent to fix this.
* Channel Lineup / EPG / Guide only works for a 24-hour period unless you restart the process (currently being fixed)
* No current way to view Tuner stats (which is tuned to what, how many bytes transferred, etc.)
* Domain and DTOs still need more cleanup and refactoring
* Unit Tests would be splendid

## Why the port of Locast2Plex?

I was having issues with that solution working properly, and have no clue how to efficiently debug Python, so it was just faster to translate it into a language / framework I'm familiar with.

## How do I use it?

### Required Environment Variables

| Variable Name | Description |
| --- | --- |
| LOCAST_USERNAME | your Locast username |
| LOCAST_PASSWORD | your Locast password |
| FFMPEG_BINARY | absolute path to a usable, local, copy of the ffmpeg binary |

### Optional Environment Variables

| Variable Name | Description | Availability |
| --- | --- | --- |
| LOCAST_ZIPCODE | bypass the normal DMA lookup process, and instead use an explicit zipcode | Working |
| LOCAST_LATITUDE | bypass the normal DMA lookup process using an explicit latitude/longitude | Working |
| LOCAST_LONGITUDE | bypass the normal DMA lookup process using an explicit latitude/longitude | Working |
| LOCAST_DMA | bypass the normal DMA lookup process using an explicit DMA | Working |
| KESTREL_LISTEN_ADDRESS | use an explicit listener address when starting the WebApi under Kestrel | Coming soon |
| KESTREL_LISTEN_PORT | use an explicit listener port when starting the WebApi under Kestrel | Coming soon |
| EPG_DAYS_TO_PULL | How many days of listings to pull at once for the Electronic Program Guide | Coming soon |

## Terminology

* DMA = Designated Market Area
* EPG = Electronic Programming Guide

## References

* [xmltv.dtd](https://github.com/XMLTV/xmltv/blob/master/xmltv.dtd)
* [XMLTV File format](http://wiki.xmltv.org/index.php/XMLTVFormat)
* [Silicondust / libhdhomerun](https://github.com/Silicondust/libhdhomerun)
* [tgorgdotcom / locast2plex](https://github.com/tgorgdotcom/locast2plex)
* [Snowman (C++ Disassembler)](https://derevenets.com/)
* [mmmmmtasty / SportScanner](https://github.com/mmmmmtasty/SportScanner)
