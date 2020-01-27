using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Extensions;

using Kontur.RetryableAssertions.Configuration;
using Kontur.RetryableAssertions.Extensions;
using Kontur.RetryableAssertions.ValueProviding;
using Kontur.Selone.Properties;

using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class AssertExtensions
    {
        public static IValueProvider<T, T> Wait<T>(this IProp<T> prop, string message = null)
        {
            return ValueProvider.Create(prop.Get, message);
        }

        public static IValueProvider<(T1, T2)[], (T1, T2)[]> Wait<T1, T2>(this IEnumerable<Props<T1, T2>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<(T1, T2, T3)[], (T1, T2, T3)[]> Wait<T1, T2, T3>(this IEnumerable<Props<T1, T2, T3>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<(T1, T2, T3, T4)[], (T1, T2, T3, T4)[]> Wait<T1, T2, T3, T4>(this IEnumerable<Props<T1, T2, T3, T4>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<(T1, T2, T3, T4, T5)[], (T1, T2, T3, T4, T5)[]> Wait<T1, T2, T3, T4, T5>(this IEnumerable<Props<T1, T2, T3, T4, T5>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<(T1, T2, T3, T4, T5, T6)[], (T1, T2, T3, T4, T5, T6)[]> Wait<T1, T2, T3, T4, T5, T6>(this IEnumerable<Props<T1, T2, T3, T4, T5, T6>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<(T1, T2, T3, T4, T5, T6, T7)[], (T1, T2, T3, T4, T5, T6, T7)[]> Wait<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<Props<T1, T2, T3, T4, T5, T6, T7>> props)
        {
            return ValueProvider.Create(props.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<T[], T[]> Wait<T>(this IEnumerable<IProp<T>> items)
        {
            return ValueProvider.Create(items.Select(x => x.Get()).ToArray, "");
        }

        public static IValueProvider<T[], T[]> Wait<T>(this IEnumerable<T> items)
        {
            return ValueProvider.Create(items.ToArray, "");
        }

        public static IAssertionResult<T, TSource> EqualTo<T, TSource>(this IValueProvider<T, TSource> provider, T value, TimeSpan? timeout = null)
        {
            return provider.That(Is.EqualTo(value), timeout);
        }

        public static IAssertionResult<T, TSource> InRange<T, TSource>(this IValueProvider<T, TSource> provider, T leftValue, T rightValue, TimeSpan? timeout = null)
        {
            return provider.That(Is.GreaterThanOrEqualTo(leftValue).And.LessThanOrEqualTo(rightValue), timeout);
        }

        public static IAssertionResult<string, TSource> Contains<TSource>(this IValueProvider<string, TSource> provider, string value, TimeSpan? timeout = null)
        {
            return provider.That(Does.Contain(value), timeout);
        }

        public static IAssertionResult<T, TSource> MoreOrEqual<T, TSource>(this IValueProvider<T, TSource> provider, T value, TimeSpan? timeout = null)
        {
            return provider.That(Is.AtLeast(value), timeout);
        }

        public static IAssertionResult<T[], TSource> EquivalentTo<T, TSource>(this IValueProvider<T[], TSource> provider, IEnumerable<T> value, TimeSpan? timeout = null)
        {
            return provider.That(Is.EquivalentTo(value), timeout);
        }

        public static IAssertionResult<T, TSource> That<T, TSource>(this IValueProvider<T, TSource> provider, IResolveConstraint resolveConstraint, TimeSpan? timeout = null)
        {
            var constraint = new ReusableConstraint(resolveConstraint);
            var assertion = Assertion.FromDelegate<T>(x =>
                {
                    using(new TestExecutionContext.IsolatedContext())
                    {
                        Assert.That(x, constraint);
                    }
                });

            return provider.Assert(assertion, GetConfiguration(timeout));
        }

        public static IAssertionResult<T, TSource> It<T, TSource>(this IValueProvider<T, TSource> provider, Action<T> assetion, TimeSpan? timeout = null)
        {
            var assertion = Assertion.FromDelegate<T>(x =>
                {
                    using(new TestExecutionContext.IsolatedContext())
                    {
                        assetion(x);
                    }
                });

            return provider.Assert(assertion, GetConfiguration(timeout));
        }

        public static void All<T, TSource>(this IValueProvider<T[], TSource> provider, Func<T, IProp<bool>> transform, TimeSpan? timeout = null)
        {
            provider.All(x => transform(x).Get().Should().BeTrue(), GetConfiguration(timeout));
        }

        public static T Single<T, TSource>(this IValueProvider<T[], TSource> provider, TimeSpan? timeout = null)
        {
            return provider.Single(GetConfiguration(timeout));
        }

        public static T Single<T, TSource, TTransformed>(this IValueProvider<T[], TSource> provider, Func<T, TTransformed> transform, IResolveConstraint resolveConstraint, TimeSpan? timeout = null)
        {
            var constraint = new ReusableConstraint(resolveConstraint);
            return provider.Single(x => Assert.That(transform(x), constraint), timeout);
        }

        public static T Single<T, TSource, TTransformed>(this IValueProvider<T[], TSource> provider, Func<T, IProp<TTransformed>> transform, IResolveConstraint resolveConstraint, TimeSpan? timeout = null)
        {
            var constraint = new ReusableConstraint(resolveConstraint);
            return provider.Single(x => Assert.That(transform(x).Get(), constraint), timeout);
        }

        public static T Single<T, TSource>(this IValueProvider<T[], TSource> provider, Action<T> assertion, TimeSpan? timeout = null)
        {
            using(new TestExecutionContext.IsolatedContext())
            {
                return provider.Single(x =>
                    {
                        using(new TestExecutionContext.IsolatedContext())
                        {
                            assertion(x);
                        }
                    }, GetConfiguration(timeout));
            }
        }

        public static IAssertionResult<T[], TSource> ShouldBeEquivalentTo<T, TSource>(
            this IValueProvider<T[], TSource> provider,
            IEnumerable<T> value,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config = null,
            TimeSpan? timeout = null)
        {
            return provider.Assert(data =>
                {
                    var sw = new Stopwatch();
                    try
                    {
                        Console.WriteLine("Start assertion");
                        sw.Start();
                        using(new TestExecutionContext.IsolatedContext())
                        {
                            if(config == null)
                            {
                                data.Should().BeEquivalentTo(value);
                            }
                            else
                            {
                                data.Should().BeEquivalentTo(value, config);
                            }
                        }
                    }
                    finally
                    {
                        sw.Stop();
                        Console.WriteLine($"Assertion: {sw.Elapsed}");
                    }
                }, GetConfiguration(timeout));
        }

        private static IAssertionConfiguration GetConfiguration(TimeSpan? timeout = null)
        {
            return new AssertionConfiguration
                {
                    Timeout = (int)(timeout ?? 10.Seconds()).TotalMilliseconds,
                    Interval = 100,
                    ExceptionMatcher = ExceptionMatcher.FromTypes(typeof(WebDriverException), typeof(PropertyTransformationException))
                };
        }
    }
}
