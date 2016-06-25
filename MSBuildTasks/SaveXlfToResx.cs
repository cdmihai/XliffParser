using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using xlflib;

namespace xlflib.MSBuildTasks
{
    public class SaveXlfToResx : Task
    {
        [Required]
        public string ResxPath { get; set; }

        [Required]
        public string XlfPath { get; set; }

        public override bool Execute()
        {
            try
            {
                var xlfDocument = new XlfDocument(XlfPath);
                xlfDocument.SaveAsResX(ResxPath);

                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to convert xlf file \"{XlfPath}\" to Resx and save it to \"{ResxPath}\"");
                Log.LogErrorFromException(e);
                return false;
            }
        }
    }
}