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
        /// Whether this parameter is static (evaluates per state) or not (evaluates per pixel) or auto -> determines automatically if true or false
        /// </summary>
        public ParameterEvaluationStrategy Evaluation { get; set; }

        /// <summary>
        /// Get the static Value
        /// </summary>
        [JsonIgnore]
        public object? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Lock to make changing the value thread safe
        /// </summary>
        public object ValueLock { get => _valueLock; }

        #endregion

        #region private fields

        // value used for static params
        private object? _value;

        // object used to lock _value
        private object _valueLock = new object();

        #endregion

        /// <summary>
        /// Creates a new StateOptionsParameter with a name and an expression
        /// </summary>
        /// <param name="name">Identifier for this parameter</param>
        /// <param name="expression">expression tto evaluate the parameter</param>
        /// <param name="pstatic">whether or not the parameter is static</param>
        public StateOptionsParameter(string name, string expression, string pstatic)
        {
            Name = name;
            Expression = expression;

            Evaluation = ParameterEvaluationStrategy.Auto;
            if (pstatic == "global") { Evaluation = ParameterEvaluationStrategy.Global; }
            else if (pstatic == "state") { Evaluation = ParameterEvaluationStrategy.PerState; }
            else if (pstatic == "pixel") { Evaluation = ParameterEvaluationStrategy.PerPixel; }
        }
    }
}
