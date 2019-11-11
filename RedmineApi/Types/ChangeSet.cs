/*
   Copyright 2012 Adrian Popescu, Dorin Huzum.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("changeset")]
    public class ChangeSet : IXmlSerializable, IEquatable<ChangeSet>
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("revision")]
        public string Revision { get; set; }

        [XmlElement("user")]
        public IdentifiableName User { get;  set; }

        [XmlElement("comments")]
        public string Comments { get; set; }

        [XmlElement("committed_on")]
        public DateTime? CommittedOn { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

            Revision = reader.GetAttribute("revision");

            reader.Read();

            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "user": User = new IdentifiableName(reader); break;

                    case "comments": Comments = reader.ReadElementContentAsString(); break;

                    case "committed_on": CommittedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteAttributeString("revision", Revision);

            if (User != null)
            {
                writer.WriteStartElement("user");
                writer.WriteAttributeString("id", User.Id.ToString());
                writer.WriteAttributeString("name", User.Name);
                writer.WriteEndElement();
            }

            writer.WriteElementString("comments", Comments);
            writer.WriteElementString("committed_on", CommittedOn.ToString());
            
        }

        public bool Equals(ChangeSet other)
        {
            if (other == null) return false;
            return Revision == other.Revision && User.Id == other.User.Id && User.Name == other.User.Name && Comments == other.Comments && CommittedOn == other.CommittedOn;
        }
    }
}