using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace BehaviourTreeTests.Utils
{
    public class ParameterNameSpecimenBuilder<T> : ISpecimenBuilder
    {
        private readonly string _parameterName;
        private readonly T _value;

        public ParameterNameSpecimenBuilder(string parameterName, T value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            _parameterName = parameterName;
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is ParameterInfo pi)) return new NoSpecimen();

            var typeMismatch = pi.ParameterType != typeof(T);
            var nameMismatch = !string.Equals(pi.Name, _parameterName, StringComparison.CurrentCultureIgnoreCase);
            if (typeMismatch || nameMismatch) return new NoSpecimen();

            return _value;
        }
    }
}