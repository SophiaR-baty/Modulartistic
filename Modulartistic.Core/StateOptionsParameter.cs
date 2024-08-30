using System.Text.Json.Serialization;

namespace Modulartistic.Core
{
    /// <summary>
    /// Parameters as defined by StateOptions
    /// </summary>
    public class StateOptionsParameter
    {
        #region public Properties

        /// <summary>
        /// Name/identifier of the Parameter
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The Expression string to evaluate the parameter
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// A value this Parameter gets initialized with. Useful for Parameters that modify itself
        /// </summary>
        public object? InitialValue { get; set; }

        /// <summary>
        /// Whether this parameter is static (evaluates per state) or not (evaluates per pixel) or auto -> determines automatically if true or false
        /// </summary>
        public ParameterEvaluationStrategy Evaluation { get; set; }

        #endregion

        /// <summary>
        /// Creates a new StateOptionsParameter with a name and an expression
        /// </summary>
        /// <param name="name">Identifier for this parameter</param>
        /// <param name="expression">expression tto evaluate the parameter</param>
        /// <param name="pstatic">whether or not the parameter is static</param>
        public StateOptionsParameter(string name, string expression, string pstatic, string initial, StateOptions sOpts, GenerationOptions gOpts)
        {
            Name = name;
            Expression = expression;

            InitialValue = null;
            if (!String.IsNullOrEmpty(initial)) 
            {
                Function f = new Function(initial);
                f.LoadAddOns(sOpts, gOpts);
                InitialValue = f.Evaluate();
            }

            Evaluation = ParameterEvaluationStrategy.Auto;
            if (pstatic == "global") { Evaluation = ParameterEvaluationStrategy.Global; }
            else if (pstatic == "object") { Evaluation = ParameterEvaluationStrategy.PerGeneration; }
            else if (pstatic == "state") { Evaluation = ParameterEvaluationStrategy.PerState; }
            else if (pstatic == "pixel") { Evaluation = ParameterEvaluationStrategy.PerPixel; }
        }
    }
}
