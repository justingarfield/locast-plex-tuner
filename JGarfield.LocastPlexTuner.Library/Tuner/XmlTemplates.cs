namespace JGarfield.LocastPlexTuner.Library.Tuner
{
    public static class XmlTemplates
    {
        public const string xmlDiscover = @"
                        <root xmlns=""urn:schemas-upnp-org:device-1-0"">
                        <specVersion>
                            <major>1</major>
                            <minor>0</minor>
                        </specVersion>
                        <device>
                            <deviceType>urn:plex-tv:device:Media:1</deviceType>
                            <friendlyName>{0}</friendlyName>
                            <manufacturer>{0}</manufacturer>
                            <manufacturerURL>https://github.com/justingarfield/LocastPlexTuner</manufacturerURL>
                            <modelName>{0}</modelName>
                            <modelNumber>{1}</modelNumber>
                            <modelDescription>{0}</modelDescription>
                            <modelURL>https://github.com/justingarfield/LocastPlexTuner</modelURL>
                            <UDN>uuid:{2}</UDN>
                            <serviceList>
                                <service>
                                    <URLBase>http://{3}</URLBase>
                                    <serviceType>urn:plex-tv:service:MediaGrabber:1</serviceType>
                                    <serviceId>urn:plex-tv:serviceId:MediaGrabber</serviceId>
                                </service>
                            </serviceList>
                        </device>
                    </root>";

        public const string xmlLineupItem = @"
                    <Program>
                        <GuideNumber>{}</GuideNumber>
                        <GuideName>{}</GuideName>
                        <URL>http://{}</URL>
                    </Program>";

        public const string xmlRmgIdentification = @"
                    <MediaContainer>
                        <MediaGrabber identifier=""tv.plex.grabbers.locastplextuner"" title=""{0}"" protocols=""livetv"" />
                    </MediaContainer>";

        public const string xmlRmgDeviceDiscover = @"
                    <MediaContainer size=""1"">
                        <Device
                            key = ""{0}""
                            make=""{1}""
                            model=""{1}"" 
                            modelNumber=""{2}"" 
                            protocol=""livetv"" 
                            status=""alive"" 
                            title=""{1}"" 
                            tuners=""{3}"" 
                            uri=""http://{4}"" 
                            uuid=""device://tv.plex.grabbers.locastplextuner/{0}"" 
                            interface=""network"" />
                    </MediaContainer>";

        public const string xmlRmgDeviceIdentity = @"
                    <MediaContainer size=""1"">
                        <Device
                            key = ""{0}""
                            make=""{1}"" 
                            model=""{1}""
                            modelNumber=""{2}""
                            protocol=""livetv"" 
                            status=""alive""
                            title=""{1} ({4})""
                            tuners=""{3}""
                            uri=""http://{4}""
                            uuid=""device://tv.plex.grabbers.locastplextuner/{0}"">
                                {5}
                        </Device>
                    </MediaContainer>";

        public const string xmlRmgTunerStreaming = @"
                    <Tuner 
                        index=""{0}""
                        status=""streaming""
                        channelIdentifier=""id://{1}""
                        lock=""1""
                        signalStrength=""100""
                        signalQuality=""100"" />";

        public const string xmlRmgTunerIdle = @"<Tuner index=""{0}"" status=""idle"" />";

        public const string xmlRmgTunerScanning = @"
                    <Tuner 
                        index=""{0}"" 
                        status=""scanning""
                        progress=""50""
                        channelsFound=""0"" />";

        public const string xmlRmgDeviceChannels = @"
                    <MediaContainer size=""{0}"">
                        {1}
                    </MediaContainer>";

        public const string xmlRmgDeviceChannelItem = @"
                    <Channel 
                        drm=""0""
                        channelIdentifier=""id://{0}""
                        name=""{1}"" 
                        origin=""LocastPlexTuner""
                        number=""{0}""
                        type=""tv"" />";

        public const string xmlRmgScanProviders = @"
                    <MediaContainer size=""1"" simultaneousScanners=""0"">
                        <Scanner type=""atsc"">
                            <Setting id=""provider"" enumValues=""1:Locast ({0})""/>
                        </Scanner>
                    </MediaContainer>";

        public const string xmlRmgScanStatus = @"<MediaContainer status=""0"" message=""Scanning..."" />";
    }
}
