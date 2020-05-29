using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;

namespace KnockKnock
{
    class Schema_Handler
    {
        //Schema field names
        public const string C_Hardware = "Hardware";
        public const string C_Tokens = "Tokens";
        public const string C_HardwareOrder = "HardwareOrder";
        public const string C_Phase = "Phase";
        public const string C_HdrFont = "HeaderFont";
        public const string C_HdrFontSize = "HeaderFontSize";
        public const string C_BdyFont = "BodyFont";
        public const string C_BdyFontSize = "BodyFontSize";

        //Note: both the GUID and schema name must change when the schema structure changes or there will be conflicts if an old schema exists.
        public Schema getSchema()
        {
            SchemaBuilder sbldr = new SchemaBuilder(new Guid("799B7C55-D6A9-42FE-93DF-425D3F4BF5CA"));
            sbldr.SetReadAccessLevel(AccessLevel.Vendor);
            sbldr.SetWriteAccessLevel(AccessLevel.Vendor);
            sbldr.SetVendorId("PKHL");
            sbldr.SetSchemaName("KnockKnockV2");

            FieldBuilder mapfield = sbldr.AddMapField(C_Hardware, typeof(string), typeof(bool));
            mapfield.SetDocumentation("A list of door hardware parameter and whether or not they are tokenized.");
			
            FieldBuilder tokenfield = sbldr.AddArrayField(C_Tokens, typeof(string));
            tokenfield.SetDocumentation("An array of allowable tokens.");
			
            FieldBuilder listfield = sbldr.AddArrayField(C_HardwareOrder, typeof(string));
            listfield.SetDocumentation("An array that holds the order the door hardware is listed.");
			
            FieldBuilder phasefield = sbldr.AddSimpleField(C_Phase, typeof(int));
            phasefield.SetDocumentation("The phase to set focus on");
			
            FieldBuilder hdrfontfield = sbldr.AddSimpleField(C_HdrFont, typeof(string));
            hdrfontfield.SetDocumentation("The name of the font to use in the column headers.");
			
            FieldBuilder hdrfontsizefield = sbldr.AddSimpleField(C_HdrFontSize, typeof(float));
            hdrfontsizefield.SetDocumentation("The size of the font to use in the column headers(in inches).");
            hdrfontsizefield.SetUnitType(UnitType.UT_Custom);
			
            FieldBuilder bdyfontfield = sbldr.AddSimpleField(C_BdyFont, typeof(string));
            bdyfontfield.SetDocumentation("The name of the font to use in the schedule body.");
			
            FieldBuilder bdyfontsizefield = sbldr.AddSimpleField(C_BdyFontSize, typeof(float));
            bdyfontsizefield.SetDocumentation("The size of the font to use in the schedule body(in inches).");
            bdyfontsizefield.SetUnitType(UnitType.UT_Custom);

            return sbldr.Finish();
        }

        public Schema getV1Schema()
        {
            //Note: both the GUID and schema name must change when the schema structure changes or there will be conflicts if an old schema exists.
            SchemaBuilder sbldr = new SchemaBuilder(new Guid("759B7C55-D6A9-42FE-93DF-425D3F4BF5CA"));
            sbldr.SetReadAccessLevel(AccessLevel.Public);
            sbldr.SetWriteAccessLevel(AccessLevel.Vendor);
            sbldr.SetVendorId("PKHL");
            sbldr.SetSchemaName("KnockKnockV1");

            FieldBuilder mapfield = sbldr.AddMapField(C_Hardware, typeof(string), typeof(bool));
            mapfield.SetDocumentation("A list of door hardware parameter and whether or not they are tokenized.");
            FieldBuilder tokenfield = sbldr.AddArrayField(C_Tokens, typeof(string));
            tokenfield.SetDocumentation("An array of allowable tokens.");
            FieldBuilder listfield = sbldr.AddArrayField(C_HardwareOrder, typeof(string));
            listfield.SetDocumentation("An array that holds the order the door hardware is listed.");
            FieldBuilder phasefield = sbldr.AddSimpleField(C_Phase, typeof(int));
            phasefield.SetDocumentation("The phase to set focus on");

            return sbldr.Finish();
        }
    }
}
