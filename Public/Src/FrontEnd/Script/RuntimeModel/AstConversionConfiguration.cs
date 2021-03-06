// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using BuildXL.Utilities.Collections;
using JetBrains.Annotations;
using BuildXL.Utilities.Configuration;

namespace BuildXL.FrontEnd.Script.RuntimeModel.AstBridge
{
    /// <summary>
    /// Configuration for AST conversion process.
    /// </summary>
    public sealed class AstConversionConfiguration
    {
        /// <nodoc/>
        public AstConversionConfiguration(
            [CanBeNull]IEnumerable<string> policyRules,
            int degreeOfParallelism,
            bool disableLanguagePolicies,
            bool useLegacyOfficeLogic)
        {
            PolicyRules = policyRules ?? CollectionUtilities.EmptyArray<string>();
            UnsafeOptions = UnsafeConversionConfiguration.GetConfigurationFromEnvironmentVariables();

            // In DScript V2 AST converter itself does not convert nodes in parallel.
            DegreeOfParalellism = useLegacyOfficeLogic ? degreeOfParallelism : 1;
            DisableLanguagePolicies = disableLanguagePolicies;
        }

        /// <nodoc />
        public static AstConversionConfiguration FromConfiguration(IFrontEndConfiguration configuration)
        {
            return new AstConversionConfiguration(
                policyRules: configuration.EnabledPolicyRules,
                degreeOfParallelism: configuration.MaxFrontEndConcurrency(),
                disableLanguagePolicies: configuration.DisableLanguagePolicyAnalysis(),
                useLegacyOfficeLogic: configuration.UseLegacyOfficeLogic())
            {
                PreserveFullNameSymbols = configuration.PreserveFullNames(),
            };
        }

        /// <summary>
        /// Retruns configuration used for converting config file.
        /// </summary>
        public static AstConversionConfiguration ForConfiguration(IFrontEndConfiguration configuration)
        {
            return new AstConversionConfiguration(
                policyRules: configuration.EnabledPolicyRules,
                degreeOfParallelism: configuration.MaxFrontEndConcurrency(),
                disableLanguagePolicies: configuration.DisableLanguagePolicyAnalysis(),
                useLegacyOfficeLogic: configuration.UseLegacyOfficeLogic())
            {
                PreserveFullNameSymbols = configuration.PreserveFullNames(),
            };
        }

        /// <nodoc/>
        [NotNull]
        public IEnumerable<string> PolicyRules { get; }

        /// <summary>
        /// If true, additional table will be used in a module literal that allows to resolve entries by a full name.
        /// </summary>
        /// <remarks>
        /// DScript V2 feature.
        /// True only for tests, because there is no other cases when this information is required.
        /// </remarks>
        public bool PreserveFullNameSymbols { get; set; }

        /// <nodoc/>
        [NotNull]
        public UnsafeConversionConfiguration UnsafeOptions { get; }

        /// <nodoc/>
        public bool ConvertInParallel => DegreeOfParalellism > 1;

        /// <nodoc/>
        public int DegreeOfParalellism { get; }

        /// <summary>
        /// Returns true if optional language policies (like required semicolons) should be disabled.
        /// </summary>
        public bool DisableLanguagePolicies { get; }
    }
}
