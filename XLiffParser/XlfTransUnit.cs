﻿namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class XlfTransUnit
    {
        public const string ResxPrefix = "Resx/";
        private const string AttributeId = "id";
        private const string ElementSource = "source";
        private const string ElementTarget = "target";
        private XElement node;
        private XNamespace ns;

        public XlfTransUnit(XElement node, XNamespace ns)
        {
            this.node = node;
            this.ns = ns;

            Optional = new Optionals(this.node, this.ns);
        }

        public XlfTransUnit(XElement node, XNamespace ns, string id, string source, string target)
            : this(node, ns)
        {
            Id = id;
            Source = source;
            Target = target;
        }

        public string Id
        {
            get { return this.node.Attribute(AttributeId).Value; }
            private set { this.node.SetAttributeValue(AttributeId, value); }
        }

        public Optionals Optional { get; }

        public string Source
        {
            get { return this.node.Element(this.ns + ElementSource).Value; }
            set { this.node.SetElementValue(this.ns + ElementSource, value); }
        }

        public string Target
        {
            get { return this.node.Element(this.ns + ElementTarget).Value; }
            set { this.node.SetElementValue(this.ns + ElementTarget, value); }
        }

        public string GetId(XlfDialect dialect)
        {
            string id = Id;
            switch (dialect)
            {
                case XlfDialect.RCWinTrans11:
                    id = Optional.Resname;
                    break;

                case XliffParser.XlfDialect.MultilingualAppToolkit:
                    if (Id.ToLowerInvariant().StartsWith(XlfTransUnit.ResxPrefix.ToLowerInvariant()))
                    {
                        id = Id.Substring(XliffParser.XlfTransUnit.ResxPrefix.Length);
                    }

                    break;

                default:
                    break;
            }

            return id;
        }

        public class Optionals
        {
            private const string AttributeApproved = "approved";
            private const string AttributeDataType = "datatype";
            private const string ElementNote = "note";
            private const string AttributeResName = "resname";
            private const string AttributeResType = "restype";
            private const string AttributeState = "state";
            private const string AttributeTranslate = "translate";
            private XElement node;
            private XNamespace ns;

            internal Optionals(XElement node, XNamespace ns)
            {
                this.node = node;
                this.ns = ns;
            }

            /// <summary>
            /// Indicates whether a translation is final or has passed its final review.
            /// </summary>
            public string Approved
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, AttributeApproved); }
                set { this.node.SetAttributeValue(AttributeApproved, value); }
            }

            /// <summary>
            /// The datatype attribute specifies the kind of text contained in the element. Depending on that type, you may
            /// apply different processes to the data. For example: datatype="winres" specifies that the content is Windows
            /// resources which would allow using the Win32 API in rendering the content.
            /// TODO later: use XlfDataType
            /// </summary>
            public string DataType
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, AttributeDataType); }
                set { this.node.SetAttributeValue(AttributeDataType, value); }
            }

            public IEnumerable<XlfNote> Notes
            {
                get
                {
                    return this.node.Descendants(this.ns + ElementNote).Select(t => new XlfNote(t));
                }
            }

            /// <summary>
            /// Resource name or identifier of a item. For example: the key in the key/value pair in a Java properties file,
            /// the ID of a string in a Windows string table, the index value of an entry in a database table, etc.
            /// </summary>
            public string Resname
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, AttributeResName); }
                set { this.node.SetAttributeValue(AttributeResName, value); }
            }

            /// <summary>
            ///  Indicates the resource type of the container element.
            /// </summary>
            public string Restype
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, AttributeResType); }
                set { this.node.SetAttributeValue(AttributeResType, value); }
            }

            /// <summary>
            /// The status of a particular translation in a <target> or <bin-target> element.
            /// <see cref="http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html#state"/>
            /// TODO later: use XlfState
            /// </summary>
            public string TargetState
            {
                get
                {
                    return XmlUtil.GetAttributeIfExists(this.node.Element(this.ns + ElementTarget), AttributeState);
                }

                set
                {
                    this.node.Element(this.ns + ElementTarget).SetAttributeValue(AttributeState, value);
                }
            }

            /// <summary>
            /// Indicates whether or not the text referred to should be translated.
            /// </summary>
            public string Translate
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, AttributeTranslate); }
                set { this.node.SetAttributeValue(AttributeTranslate, value); }
            }

            public void AddNote(string comment, string from)
            {
                var note = new XlfNote(new XElement(this.ns + ElementNote, comment));
                if (!string.IsNullOrWhiteSpace(from))
                {
                    note.Optional.From = from;
                }

                this.node.Add(note.GetNode());
            }

            public void AddNote(string comment)
            {
                AddNote(comment, string.Empty);
            }

            public void SetCommentFromResx(string comment)
            {
                if (Notes.Any())
                {
                    Notes.First().Value = comment;
                }
                else
                {
                    AddNote(comment);
                }
            }

            public void RemoveNotes(string attributeName, string value)
            {
                this.node.Descendants(this.ns + ElementNote).Where(u =>
                {
                    var a = u.Attribute(attributeName);
                    return a != null && a.Value == value;
                }).Remove();
            }

            public override string ToString()
            {
                return node.ToString();
            }
        }
    }
}