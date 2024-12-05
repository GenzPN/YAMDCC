// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YAMDCC.Config
{
    /// <summary>
    /// Represents a YAMDCC configuration.
    /// </summary>
    public sealed class YAMDCC_Config
    {
        /// <summary>
        /// The config version expected when loading a config.
        /// </summary>
        [XmlIgnore]
        public const int ExpectedVer = 1;

        /// <summary>
        /// The config version. Should be the same as <see cref="ExpectedVer"/>
        /// unless the config is newer or invalid.
        /// </summary>
        [XmlAttribute]
        public int Ver { get; set; }

        /// <summary>
        /// The laptop model the config was made for.
        /// </summary>
        [XmlElement]
        public string Model { get; set; }

        /// <summary>
        /// The author of the config file.
        /// </summary>
        [XmlElement]
        public string Author { get; set; }

        /// <summary>
        /// The list of <see cref="FanConf"/>s associated with the laptop.
        /// </summary>
        [XmlArray]
        public FanConf[] FanConfs { get; set; }

        /// <summary>
        /// The laptop's Full Blast config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if not supported on the laptop.
        /// </remarks>
        [XmlElement]
        public FullBlastConf FullBlastConf { get; set; }

        /// <summary>
        /// The laptop's charge threshold config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if not supported on the laptop.
        /// </remarks>
        [XmlElement]
        public ChargeLimitConf ChargeLimitConf { get; set; }

        /// <summary>
        /// The laptop's performance mode config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if not supported on the laptop.
        /// </remarks>
        [XmlElement]
        public PerfModeConf PerfModeConf { get; set; }

        /// <summary>
        /// The laptop's Win/Fn keyboard swap config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if not supported on the laptop.
        /// </remarks>
        [XmlElement]
        public KeySwapConf KeySwapConf { get; set; }

        /// <summary>
        /// The laptop's keyboard backlight config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c> if not supported on the laptop.
        /// </remarks>
        [XmlElement]
        public KeyLightConf KeyLightConf { get; set; }

        /// <summary>
        /// A list of registers to write when applying a fan config.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>, but if not <c>null</c>, must have
        /// at least one <see cref="RegConf"/>.
        /// </remarks>
        [XmlArray]
        public RegConf[] RegConfs { get; set; }

        /// <summary>
        /// Parses a YAMDCC config XML and returns an
        /// <see cref="YAMDCC_Config"/> object.
        /// </summary>
        /// <param name="xmlFile">The path to an XML config file.</param>
        /// <exception cref="InvalidConfigException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="InvalidOperationException"/>
        public static YAMDCC_Config Load(string xmlFile)
        {
            XmlSerializer serialiser = new(typeof(YAMDCC_Config));
            using (XmlReader reader = XmlReader.Create(xmlFile))
            {
                YAMDCC_Config cfg = (YAMDCC_Config)serialiser.Deserialize(reader);
                return cfg.IsValid() ? cfg : throw new InvalidConfigException();
            }
        }

        /// <summary>
        /// Saves a YAMDCC config to the specified location.
        /// </summary>
        /// <param name="xmlFile">The XML file to write to.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Save(string xmlFile)
        {
            XmlSerializer serializer = new(typeof(YAMDCC_Config));
            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = "\t",
            };

            using (XmlWriter writer = XmlWriter.Create(xmlFile, settings))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Performs some validation on the loaded config to make
        /// sure it is in the expected format.
        /// </summary>
        /// <remarks>
        /// This does NOT guarantee the loaded config is valid!
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the config is valid, otherwise <c>false</c>.
        /// </returns>
        private bool IsValid()
        {
            // Check the config version.
            // Pretty self-explanatory, if the loaded config is older/newer
            // than the version expected by the config library, don't bother
            // checking anything else as some/all of it is probably invalid.
            if (Ver != ExpectedVer)
            {
                return false;
            }

            if (string.IsNullOrEmpty(Model) ||
                string.IsNullOrEmpty(Author))
            {
                return false;
            }

            // 1. Check if FanConfigs is not null
            // 2. Check if there's at least 1 FanConfig
            if (FanConfs?.Length >= 1)
            {
                for (int i = 0; i < FanConfs.Length; i++)
                {
                    FanConf cfg = FanConfs[i];

                    if (string.IsNullOrEmpty(cfg.Name))
                    {
                        return false;
                    }

                    // the selected fan curve shouldn't be higher than
                    // the number of fan curves in the config.
                    if (cfg.CurveSel >= FanConfs[i].FanCurveConfs.Length ||
                        cfg.CurveSel < 0)
                    {
                        // if the fan profile selection is out of range,
                        // silently set it to 0 (the first fan curve)
                        // which should always exist:
                        cfg.CurveSel = 0;
                    }

                    if (cfg.UpThresholdRegs?.Length >= 1 &&
                        cfg.DownThresholdRegs?.Length >= 1 &&
                        cfg.FanCurveRegs?.Length >= 2 &&
                        cfg.FanCurveConfs?.Length >= 1)
                    {
                        for (int j = 0; j < cfg.FanCurveConfs.Length; j++)
                        {
                            FanCurveConf curveCfg = cfg.FanCurveConfs[j];
                            if (string.IsNullOrEmpty(curveCfg.Name) ||
                                string.IsNullOrEmpty(curveCfg.Desc) ||
                                // there should be exactly one temperature threshold
                                // per fan curve register; if there isn't, return false
                                curveCfg.TempThresholds?.Length != cfg.FanCurveRegs.Length)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            // If the RegConfigs tag is defined in the XML,
            // but has no elements, return false
            if (RegConfs?.Length == 0)
            {
                return false;
            }

            if (PerfModeConf is not null)
            {
                // the selected performance mode shouldn't be higher than
                // the number of performance modes in the config
                if (PerfModeConf.ModeSel >= PerfModeConf.PerfModes.Length ||
                    PerfModeConf.ModeSel < 0)
                {
                    return false;
                }
            }

            // All other values are considered to be valid; return true
            return true;
        }
    }
}
