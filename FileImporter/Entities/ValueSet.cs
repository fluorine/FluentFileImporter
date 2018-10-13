using System;
using System.Collections.Generic;
using System.Text;

namespace FileImporter.Example.Entities
{
    public class ValueSet
    {
        public string ValueSetName { get; set; }
        public string ValueSetOid { get; set; }
        public string DefinitionVersion { get; set; }
        public string ExpansionVersion { get; set; }
        public string PurposeClinicalFocus { get; set; }
        public string PurposeDataElementScope { get; set; }
        public string PurposeInclusionCriteria { get; set; }
        public string PurposeExclusionCriteria { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeSystem { get; set; }
        public string CodeSystemOID { get; set; }
        public string CodeSystemVersion { get; set; }

        public override string ToString()
        {
            return $"{ValueSetName} ({ValueSetOid})\n" +
                $" - Code:        {Code} ({CodeSystem})\n" +
                $" - Description: {Description}\n\n";
        }
    }
}
