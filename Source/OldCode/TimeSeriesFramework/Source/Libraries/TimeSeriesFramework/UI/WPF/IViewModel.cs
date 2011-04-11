﻿//******************************************************************************************************
//  IViewModel.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  03/31/2011 - Mehulbhai P Thakkar
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Windows.Input;

namespace TimeSeriesFramework.UI
{
    /// <summary>
    /// Represents an interface and common methods and properties each data model definition should use.
    /// </summary>
    public interface IViewModel
    {
        #region [ Properties ]

        /// <summary>
        /// Gets <see cref="ICommand"/> object to perform Save operation.
        /// </summary>
        ICommand SaveCommand { get; }

        /// <summary>
        /// Gets <see cref="ICommand"/> object to perform Delete operation.
        /// </summary>
        ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets <see cref="ICommand"/> object to perform Clear operation.
        /// </summary>
        ICommand ClearCommand { get; }

        /// <summary>
        /// Gets boolean value to indicate if Save operation can be executed.
        /// </summary>
        /// <returns></returns>
        bool CanSave { get; }

        /// <summary>
        /// Gets boolean value to indicate if Delete operation can be executed.
        /// </summary>
        /// <returns></returns>
        bool CanDelete { get; }

        /// <summary>
        /// Gets boolean value to indicate if Clear operation can be executed.
        /// </summary>
        /// <returns></returns>
        bool CanClear { get; }             

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Method to save data into backend data store.
        /// </summary>
        void Save();

        /// <summary>
        /// Method to delete data from the backend data store.
        /// </summary>
        void Delete();

        /// <summary>
        /// Method to restore default values for data model object.
        /// </summary>
        void Clear();

        /// <summary>
        /// Method to retrieve collection of related objects.
        /// </summary>
        void Load();

        #endregion
    }
}
