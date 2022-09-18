using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace BehaviourTreeTests.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() : base(CreateFixture) {}

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            return fixture;
        }
    }
}