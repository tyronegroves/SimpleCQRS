using System;

#region License

//
// Author: Javier Lozano <javier@lozanotek.com>
// Copyright (c) 2009-2010, lozanotek, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

namespace SimpleCqrs
{
    [Serializable]
    public class ServiceResolutionException : Exception
    {
        public ServiceResolutionException(Type service) :
            base(string.Format("Could not resolve serviceType '{0}'", service))
        {
            ServiceType = service;
        }

        public ServiceResolutionException(Type service, Exception innerException)
            : base(string.Format("Could not resolve serviceType '{0}'", service), innerException)
        {
            ServiceType = service;
        }

        public Type ServiceType { get; set; }
    }
}