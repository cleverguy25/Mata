// <copyright file="ParameterSet.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Mata;

    public class ParameterSet
    {
        private readonly Dictionary<string, IDataParameter> parameters;

        public ParameterSet(IList originalParameters)
        {
            this.parameters = ConvertParametersToDictionary(originalParameters);
        }

        public void SetParameter(IDbCommand command, string parameterName, object value)
        {
            IDataParameter parameter;
            if (this.parameters.TryGetValue(parameterName.ToLower(), out parameter) == false)
            {
                return;
            }

            var newParameter = CopyParameter(parameter);
            newParameter.Value = value;
            command.Parameters.Add(newParameter);
        }

        private static IDataParameter CopyParameter(object parameter)
        {
#if NETSTANDARD1_6
            return Cloner.CloneWithIL((IDataParameter)parameter);
#else
            return (IDataParameter)((ICloneable)parameter).Clone();
#endif
        }

        private static Dictionary<string, IDataParameter> ConvertParametersToDictionary(IList originalParameters)
        {
            var count = originalParameters.Count;
            var dictionary = new Dictionary<string, IDataParameter>(count);
            foreach (var parameter in originalParameters)
            {
                var newParameter = CopyParameter(parameter);
                dictionary[newParameter.ParameterName.ToLower()] = newParameter;
            }

            return dictionary;
        }
    }
}