﻿// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System.Xml.Serialization;

namespace MSIFanControl.Config
{
    /// <summary>
    /// Represents miscellaneous register configurations for the target
    /// system. May be required to enable fan control via MSI Fan Control.
    /// </summary>
    /// <remarks>
    /// All RegConfigs defined here will be applied on service start.
    /// </remarks>
    public sealed class RegConfig
    {
        /// <summary>
        /// A description of what this config does.
        /// </summary>
        [XmlElement]
        public string Description;

        /// <summary>
        /// The register to write to.
        /// </summary>
        [XmlElement]
        public byte Register;

        /// <summary>
        /// The value to write to the register.
        /// </summary>
        [XmlElement]
        public byte Value;
    }
}
