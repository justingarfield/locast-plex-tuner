# LocastPlexTuner

## Overview

This is a full port of the https://github.com/tgorgdotcom/locast2plex Python library.

## Why?

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
| LOCAST_LATITUDE | bypass the normal DMA lookup process using an explicit latitude/longitude | Coming soon |
| LOCAST_LONGITUDE | bypass the normal DMA lookup process using an explicit latitude/longitude | Coming soon |
| LOCAST_DMA | bypass the normal DMA lookup process using an explicit DMA | Coming soon |
| KESTREL_LISTEN_ADDRESS | use an explicit listener address when starting the WebApi under Kestrel | Coming soon |
| KESTREL_LISTEN_PORT | use an explicit listener port when starting the WebApi under Kestrel | Coming soon |

## Terminology

* DMA = Designated Market Area
* EPG = Electronic Programming Guide
