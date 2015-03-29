using ILTools.Contracts;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILTools.MsBuild
{
    public class AssemblyRewrite : Task
    {
        [Required]
        public string AssemblyPath { get; set; }

        public override bool Execute()
        {
            //Log.LogError("AssemblyPath: {0}", AssemblyPath);

            var rewriter = new AssemblyRewriter(AssemblyPath);

            rewriter.MethodRewriters.Add(new NotNullAttributeRewriter());

            rewriter.RewriteAndSave(AssemblyPath);

            return true;
        }
    }
}
