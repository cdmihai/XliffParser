using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using xlflib;

namespace xlflib.MSBuildTasks
{
    public class UpdateXlfFromResx : Task
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
                Tuple<int, int, int> result = xlfDocument.UpdateFromResX(ResxPath);

                Log.LogMessage(MessageImportance.Low,
                                "Update results:" +
                                $"\n\t{result.Item1} resources updated" +
                                $"\n\t{result.Item2} resources added" +
                                $"\n\t{result.Item3} resources deleted");

                xlfDocument.Save();

                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to update xlf file \"{XlfPath}\" from \"{ResxPath}\"");
                Log.LogErrorFromException(e, true);
                return false;
            }
        }
    }
}