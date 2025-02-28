﻿using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace Wabbajack.Lib.CompilationSteps
{
    public class IncludeModIniData : ACompilationStep
    {
        public IncludeModIniData(Compiler compiler) : base(compiler)
        {
        }

        public override Directive Run(RawSourceFile source)
        {
            if (!source.Path.StartsWith("mods\\") || !source.Path.EndsWith("\\meta.ini")) return null;
            var e = source.EvolveTo<InlineFile>();
            e.SourceDataID = _compiler.IncludeFile(File.ReadAllBytes(source.AbsolutePath));
            return e;
        }

        public override IState GetState()
        {
            return new State();
        }

        [JsonObject("IncludeModIniData")]
        public class State : IState
        {
            public ICompilationStep CreateStep(Compiler compiler)
            {
                return new IncludeModIniData(compiler);
            }
        }
    }
}