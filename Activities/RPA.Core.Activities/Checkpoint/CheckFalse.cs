using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.ComponentModel;

namespace RPA.Core.Activities.Checkpoint
{
    [Designer(typeof(CheckFalseDesigner))]
    public class CheckFalse:Activity
    {

        [RequiredArgument]
        [Description("要评估的表达式。")]
        public InArgument<bool> Expression
        {
            get;
            set;
        }

        [RequiredArgument]
        [Description("如果表达式不正确，将显示错误消息。")]
        public InArgument<string> ErrorMessage
        {
            get;
            set;
        }
        public  CheckFalse()
        {
            this.Implementation = delegate
            {
                return new If
                {
                    Condition = new ArgumentValue<bool>("Expression"),
                    Then = new Throw
                    {
                        DisplayName = base.DisplayName,
                        Exception = new LambdaValue<Exception>((ActivityContext context) => new CheckpointException(this.ErrorMessage.Get(context)))
                    }
                };
            };
        }
    } 
}
