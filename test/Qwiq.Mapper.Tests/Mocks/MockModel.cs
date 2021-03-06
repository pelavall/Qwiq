using System;

using Qwiq.Mapper.Attributes;

namespace Qwiq.Mapper.Mocks
{
    [WorkItemType("MockWorkItem")]
    public class MockModel : IIdentifiable<int?>
    {
        [FieldDefinition("Id")]
        public virtual int? Id { get; internal set; }

        [FieldDefinition("IntField")]
        public int IntField { get; internal set; }

        [FieldDefinition("DateTimeField")]
        public DateTime DateTimeField { get; internal set; }

        [FieldDefinition("StringField")]
        public virtual string StringField { get; internal set; }

        [FieldDefinition("NullableField")]
        public int? NullableField { get; internal set; }

        [FieldDefinition("Field with Spaces")]
        public string FieldWithSpaces { get; internal set; }

        [FieldDefinition("FieldWithDifferentName")]
        public string NotTheSameName { get; internal set; }

        public DateTime? UnmappedProperty { get; internal set; }
    }
}

