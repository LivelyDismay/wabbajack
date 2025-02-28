﻿using Newtonsoft.Json;

namespace Wabbajack.Lib.CompilationSteps
{
    public class IgnoreStartsWith : ACompilationStep
    {
        private readonly string _prefix;
        private readonly string _reason;

        public IgnoreStartsWith(Compiler compiler, string prefix) : base(compiler)
        {
            _prefix = prefix;
            _reason = string.Format("Ignored because path starts with {0}", _prefix);
        }

        public override Directive Run(RawSourceFile source)
        {
            if (source.Path.StartsWith(_prefix))
            {
                var result = source.EvolveTo<IgnoredDirectly>();
                result.Reason = _reason;
                return result;
            }

            return null;
        }

        public override IState GetState()
        {
            return new State(_prefix);
        }

        [JsonObject("IgnoreStartsWith")]
        public class State : IState
        {
            public State()
            {
            }

            public State(string prefix)
            {
                Prefix = prefix;
            }

            public string Prefix { get; set; }

            public ICompilationStep CreateStep(Compiler compiler)
            {
                return new IgnoreStartsWith(compiler, Prefix);
            }
        }
    }
}