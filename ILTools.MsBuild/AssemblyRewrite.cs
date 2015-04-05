﻿using ILTools.MethodProcessors;
using ILTools.MethodProcessors.Contracts;
using ILTools.MethodProcessors.Helpers;
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
            var logger = new MsBuildLogger(Log);
            return Execute(AssemblyPath, logger);
        }

        public bool Execute(string outputPath, ILogger logger = null)
        {
            var rewriter = new AssemblyRewriter(AssemblyPath, logger);

            //rewriter.MethodProcessors.Add(new NotNullAttributeProcessor2());
            rewriter.MethodProcessors.Add(new MakeStaticVersionProcessor());

            rewriter.ProcessAssemblyAndSave(outputPath);

            return true;
        }
    }
}
