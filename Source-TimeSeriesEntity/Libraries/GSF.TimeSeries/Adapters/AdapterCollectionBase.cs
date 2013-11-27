﻿//******************************************************************************************************
//  AdapterCollectionBase.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  09/02/2010 - J. Ritchie Carroll
//       Generated original version of source code.
//  11/04/2013 - Stephen C. Wills
//       Updated to process time-series entities.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using GSF.IO;
using GSF.Units;
using Timer = System.Timers.Timer;

namespace GSF.TimeSeries.Adapters
{
    /// <summary>
    /// Represents a collection of <see cref="IAdapter"/> implementations.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IAdapter"/> this collection contains.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AdapterCollectionBase<T> : Collection<T>, IAdapterCollection where T : class, IAdapter
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Provides status messages to consumer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="EventArgs{T}.Argument"/> is new status message.
        /// </para>
        /// <para>
        /// EventHander sender object will be represent source adapter or this collection.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs<string>> StatusMessage;

        /// <summary>
        /// Event is raised when there is an exception encountered while processing.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="EventArgs{T}.Argument"/> is the exception that was thrown.
        /// </para>
        /// <para>
        /// EventHander sender object will be represent source adapter or this collection.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs<Exception>> ProcessException;

        /// <summary>
        /// Event is raised when <see cref="InputSignals"/> are updated in any of the adapters in the collection.
        /// </summary>
        /// <remarks>
        /// EventHander sender object will be represent source adapter.
        /// </remarks>
        public event EventHandler InputSignalsUpdated;

        /// <summary>
        /// Event is raised when <see cref="OutputSignals"/> are updated in any of the adapters in the collection.
        /// </summary>
        /// <remarks>
        /// EventHander sender object will be represent source adapter.
        /// </remarks>
        public event EventHandler OutputSignalsUpdated;

        /// <summary>
        /// Event is raised when an adapter is aware of a configuration change.
        /// </summary>
        /// <remarks>
        /// EventHander sender object will be represent source adapter.
        /// </remarks>
        public event EventHandler ConfigurationChanged;

        /// <summary>
        /// This event is raised if there are any time-series entities being discarded during processing in any of the adapters in the collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="EventArgs{T}.Argument"/> is the enumeration of <see cref="ITimeSeriesEntity"/> objects that are being discarded during processing.
        /// </para>
        /// <para>
        /// EventHander sender object will be represent source adapter.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs<IEnumerable<ITimeSeriesEntity>>> EntitiesDiscarded;

        /// <summary>
        /// Event is raised when this <see cref="AdapterCollectionBase{T}"/> is disposed or an <see cref="IAdapter"/> in the collection is disposed.
        /// </summary>
        /// <remarks>
        /// EventHander sender object will be represent source adapter or this collection.
        /// </remarks>
        public event EventHandler Disposed;

        // Fields
        private string m_name;
        private uint m_id;
        private bool m_initialized;
        private DataSet m_dataSource;
        private string m_dataMember;
        private readonly bool m_temporalCollection;
        private Ticks m_lastProcessTime;
        private Time m_totalProcessTime;
        private long m_processedEntities;
        private int m_processingInterval;
        private Timer m_monitorTimer;
        private bool m_monitorTimerEnabled;
        private bool m_delayAutoStart;
        private bool m_enabled;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Constructs a new instance of the <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        /// <param name="temporalCollection">Determines if this collection is being used in a temporal <see cref="IaonSession"/>.</param>
        protected AdapterCollectionBase(bool temporalCollection)
        {
            m_name = GetType().Name;
            m_processingInterval = -1;
            m_temporalCollection = temporalCollection;

            m_monitorTimer = new Timer();
            m_monitorTimer.Elapsed += m_monitorTimer_Elapsed;

            // We monitor total number of processed entities every minute
            m_monitorTimer.Interval = 60000;
            m_monitorTimer.AutoReset = true;
            m_monitorTimer.Enabled = false;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="AdapterCollectionBase{T}"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~AdapterCollectionBase()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets numeric ID associated with this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        public virtual uint ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// Gets or sets flag indicating if the adapter collection has been initialized successfully.
        /// </summary>
        public virtual bool Initialized
        {
            get
            {
                return m_initialized;
            }
            set
            {
                m_initialized = value;
            }
        }

        /// <summary>
        /// Gets or sets flag that determines if adapters should delay auto-start until after initialization.
        /// </summary>
        public virtual bool DelayAutoStart
        {
            get
            {
                return m_delayAutoStart;
            }
            set
            {
                m_delayAutoStart = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="DataSet"/> based data source used to load each <see cref="IAdapter"/>.
        /// Updates to this property will cascade to all items in this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        /// <remarks>
        /// Table name specified in <see cref="DataMember"/> from <see cref="DataSource"/> is expected
        /// to have the following table column names:<br/>
        /// ID, AdapterName, AssemblyName, TypeName, ConnectionString<br/>
        /// ID column type should be integer based, all other column types are expected to be strings.
        /// </remarks>
        public virtual DataSet DataSource
        {
            get
            {
                return m_dataSource;
            }
            set
            {
                m_dataSource = value;

                // Update data source for items in this collection
                lock (this)
                {
                    foreach (T item in this)
                    {
                        item.DataSource = m_dataSource;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets specific data member (e.g., table name) in <see cref="DataSource"/> used to <see cref="Initialize"/> this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        /// <remarks>
        /// Table name specified in <see cref="DataMember"/> from <see cref="DataSource"/> is expected
        /// to have the following table column names:<br/>
        /// ID, AdapterName, AssemblyName, TypeName, ConnectionString<br/>
        /// ID column type should be integer based, all other column types are expected to be strings.
        /// </remarks>
        public virtual string DataMember
        {
            get
            {
                return m_dataMember;
            }
            set
            {
                m_dataMember = value;
            }
        }

        /// <summary>
        /// Gets flag that determines if this collection is being used in a temporal <see cref="IaonSession"/>.
        /// </summary>
        public bool TemporalCollection
        {
            get
            {
                return m_temporalCollection;
            }
        }

        /// <summary>
        /// Gets or sets primary keys of input signals the <see cref="AdapterCollectionBase{T}"/> expects, if any.
        /// </summary>
        public virtual ISet<Guid> InputSignals
        {
            get
            {
                ISet<Guid> cumulativeSignals = new HashSet<Guid>();

                // Cumulate results of all child adapters
                lock (this)
                {
                    foreach (T adapter in this)
                    {
                        if ((object)adapter != null)
                            cumulativeSignals.UnionWith(adapter.InputSignals);
                    }
                }

                return cumulativeSignals;
            }
        }

        /// <summary>
        /// Gets or sets output signals that the <see cref="AdapterCollectionBase{T}"/> will produce, if any.
        /// </summary>
        public virtual ISet<Guid> OutputSignals
        {
            get
            {
                ISet<Guid> cumulativeSignals = new HashSet<Guid>();

                // Cumulate results of all child adapters
                lock (this)
                {
                    foreach (T adapter in this)
                    {
                        if ((object)adapter != null)
                            cumulativeSignals.UnionWith(adapter.OutputSignals);
                    }
                }

                return cumulativeSignals;
            }
        }

        /// <summary>
        /// Gets or sets the desired processing interval, in milliseconds, for the adapter collection and applies this interval to each adapter.
        /// </summary>
        /// <remarks>
        /// With the exception of the values of -1 and 0, this value specifies the desired processing interval for data, i.e.,
        /// basically a delay, or timer interval, over which to process data. A value of -1 means to use the default processing
        /// interval while a value of 0 means to process data as fast as possible.
        /// </remarks>
        public virtual int ProcessingInterval
        {
            get
            {
                return m_processingInterval;
            }
            set
            {
                m_processingInterval = value;

                if (m_processingInterval < -1)
                    m_processingInterval = -1;

                // Apply this new processing interval for all adapters in the collection
                lock (this)
                {
                    foreach (T item in this)
                        item.ProcessingInterval = m_processingInterval;
                }
            }
        }

        /// <summary>
        /// Gets the total number of time-series entities processed thus far by each <see cref="IAdapter"/> implementation
        /// in the <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        public virtual long ProcessedEntities
        {
            get
            {
                long processedEntities = 0;

                // Calculate new total for all adapters
                lock (this)
                {
                    foreach (T item in this)
                        processedEntities += item.ProcessedEntities;
                }

                return processedEntities;
            }
        }

        /// <summary>
        /// Gets or sets enabled state of this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                if (m_enabled && !value)
                    Stop();
                else if (!m_enabled && value)
                    Start();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AdapterCollectionBase{T}"/> is read-only.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets flag that determines if monitor timer should be used for monitoring processed entity statistics for the <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        protected virtual bool MonitorTimerEnabled
        {
            get
            {
                return m_monitorTimerEnabled;
            }
            set
            {
                m_monitorTimerEnabled = value && Enabled;

                if ((object)m_monitorTimer != null)
                    m_monitorTimer.Enabled = m_monitorTimerEnabled;
            }
        }

        /// <summary>
        /// Gets flag that determines if <see cref="IAdapter"/> implementations are automatically initialized
        /// when they are added to the collection.
        /// </summary>
        protected virtual bool AutoInitialize
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the descriptive status of this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        public virtual string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                DataSet dataSource = DataSource;

                // Show collection status
                status.AppendFormat("  Total adapter components: {0}", Count);
                status.AppendLine();
                status.AppendFormat("    Collection initialized: {0}", Initialized);
                status.AppendLine();
                status.AppendFormat(" Current operational state: {0}", (Enabled ? "Enabled" : "Disabled"));
                status.AppendLine();
                if (MonitorTimerEnabled)
                {
                    status.AppendFormat("        Processed entities: {0}", m_processedEntities.ToString("N0"));
                    status.AppendLine();
                    status.AppendFormat("   Average processing rate: {0} entities / second", ((int)(m_processedEntities / m_totalProcessTime)).ToString("N0"));
                    status.AppendLine();
                }
                status.AppendFormat("       Data source defined: {0}", ((object)dataSource != null));
                status.AppendLine();
                if (dataSource != null)
                {
                    status.AppendFormat("    Referenced data source: {0}, {1} tables", dataSource.DataSetName, dataSource.Tables.Count);
                    status.AppendLine();
                }
                status.AppendFormat("    Data source table name: {0}", DataMember);
                status.AppendLine();

                if (Count > 0)
                {
                    int index = 0;

                    status.AppendLine();
                    status.AppendFormat("Status of each {0} component:", Name);
                    status.AppendLine();
                    status.Append(new string('-', 79));
                    status.AppendLine();

                    // Show the status of registered components.
                    lock (this)
                    {
                        foreach (T item in this)
                        {
                            IProvideStatus statusProvider = item as IProvideStatus;

                            if (statusProvider != null)
                            {
                                // This component provides status information.                       
                                status.AppendLine();
                                status.AppendFormat("Status of {0} component {1}, {2}:", typeof(T).Name, ++index, statusProvider.Name);
                                status.AppendLine();
                                try
                                {
                                    status.Append(statusProvider.Status);
                                }
                                catch (Exception ex)
                                {
                                    status.AppendFormat("Failed to retrieve status due to exception: {0}", ex.Message);
                                    status.AppendLine();
                                }
                            }
                        }
                    }

                    status.AppendLine();
                    status.Append(new string('-', 79));
                    status.AppendLine();
                }

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="AdapterCollectionBase{T}"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="AdapterCollectionBase{T}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (m_monitorTimer != null)
                        {
                            m_monitorTimer.Elapsed -= m_monitorTimer_Elapsed;
                            m_monitorTimer.Dispose();
                        }
                        m_monitorTimer = null;

                        Clear();        // This disposes all items in collection...
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.

                    if (Disposed != null)
                        Disposed(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Loads all <see cref="IAdapter"/> implementations defined in <see cref="DataSource"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Table name specified in <see cref="DataMember"/> from <see cref="DataSource"/> is expected
        /// to have the following table column names:<br/>
        /// ID, AdapterName, AssemblyName, TypeName, ConnectionString<br/>
        /// ID column type should be integer based, all other column types are expected to be string based.
        /// </para>
        /// <para>
        /// Note that when calling this method any existing items will be cleared allowing a "re-initialize".
        /// </para>
        /// </remarks>
        /// <exception cref="NullReferenceException">DataSource is null.</exception>
        /// <exception cref="InvalidOperationException">DataMember is null or empty.</exception>
        public virtual void Initialize()
        {
            T item;

            if ((object)DataSource == null)
                throw new NullReferenceException(string.Format("DataSource is null, cannot load {0}", Name));

            if (string.IsNullOrWhiteSpace(DataMember))
                throw new InvalidOperationException(string.Format("DataMember is null or empty, cannot load {0}", Name));

            Initialized = false;

            // Load the default initialization parameter for adapters in this collection
            lock (this)
            {
                Clear();

                if (DataSource.Tables.Contains(DataMember))
                {
                    DataRow[] rows;

                    if (m_temporalCollection)
                        rows = DataSource.Tables[DataMember].Select("TemporalSession <> 0");
                    else
                        rows = DataSource.Tables[DataMember].Select();

                    foreach (DataRow adapterRow in rows)
                    {
                        if (TryCreateAdapter(adapterRow, out item))
                            Add(item);
                    }

                    Initialized = true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Data set member \"{0}\" was not found in data source, check ConfigurationEntity. Failed to initialize {1}.", DataMember, Name));
                }
            }
        }

        /// <summary>
        /// Attempts to create an <see cref="IAdapter"/> from the specified <see cref="DataRow"/>.
        /// </summary>
        /// <param name="adapterRow"><see cref="DataRow"/> containing item information to initialize.</param>
        /// <param name="adapter">Initialized adapter if successful; otherwise null.</param>
        /// <returns><c>true</c> if item was successfully initialized; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// See <see cref="DataSource"/> property for expected <see cref="DataRow"/> column names.
        /// </remarks>
        /// <exception cref="NullReferenceException"><paramref name="adapterRow"/> is null.</exception>
        protected virtual bool TryCreateAdapter(DataRow adapterRow, out T adapter)
        {
            if (adapterRow == null)
                throw new NullReferenceException(string.Format("Cannot initialize from null adapter DataRow"));

            Assembly assembly;
            string name = "", assemblyName = "", typeName = "", connectionString, setting;
            uint id;

            try
            {
                name = adapterRow["AdapterName"].ToNonNullString("[IAdapter]");
                assemblyName = FilePath.GetAbsolutePath(adapterRow["AssemblyName"].ToNonNullString());
                typeName = adapterRow["TypeName"].ToNonNullString();
                connectionString = adapterRow["ConnectionString"].ToNonNullString();
                id = uint.Parse(adapterRow["ID"].ToNonNullString("0"));

                if (string.IsNullOrWhiteSpace(typeName))
                    throw new InvalidOperationException("No adapter type was defined");

                if (!File.Exists(assemblyName))
                    throw new InvalidOperationException("Specified adapter assembly does not exist");

                assembly = Assembly.LoadFrom(assemblyName);
                adapter = (T)Activator.CreateInstance(assembly.GetType(typeName));

                // Assign critical adapter properties
                adapter.Name = name;
                adapter.ID = id;
                adapter.ConnectionString = connectionString;
                adapter.DataSource = DataSource;

                // Assign adapter initialization timeout   
                if (adapter.Settings.TryGetValue("initializationTimeout", out setting))
                    adapter.InitializationTimeout = int.Parse(setting);
                else
                    adapter.InitializationTimeout = AdapterBase.DefaultInitializationTimeout;

                return true;
            }
            catch (Exception ex)
            {
                // We report any errors encountered during type creation...
                OnProcessException(new InvalidOperationException(string.Format("Failed to load adapter \"{0}\" [{1}] from \"{2}\": {3}", name, typeName, assemblyName, ex.Message), ex));
            }

            adapter = default(T);
            return false;
        }

        // Explicit IAdapter implementation of TryCreateAdapter
        bool IAdapterCollection.TryCreateAdapter(DataRow adapterRow, out IAdapter adapter)
        {
            T adapterT;
            bool result = TryCreateAdapter(adapterRow, out adapterT);
            adapter = adapterT as IAdapter;
            return result;
        }

        /// <summary>
        /// Attempts to get the adapter with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of adapter to get.</param>
        /// <param name="adapter">Adapter reference if found; otherwise null.</param>
        /// <returns><c>true</c> if adapter with the specified <paramref name="id"/> was found; otherwise <c>false</c>.</returns>
        public virtual bool TryGetAdapterByID(uint id, out T adapter)
        {
            return TryGetAdapter(id, (item, value) => item.ID == value, out adapter);
        }

        /// <summary>
        /// Attempts to get the adapter with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of adapter to get.</param>
        /// <param name="adapter">Adapter reference if found; otherwise null.</param>
        /// <returns><c>true</c> if adapter with the specified <paramref name="name"/> was found; otherwise <c>false</c>.</returns>
        public virtual bool TryGetAdapterByName(string name, out T adapter)
        {
            return TryGetAdapter(name, (item, value) => string.Compare(item.Name, value, true) == 0, out adapter);
        }

        /// <summary>
        /// Attempts to get the adapter with the specified <paramref name="value"/> given <paramref name="testItem"/> function.
        /// </summary>
        /// <param name="value">Value of adapter to get.</param>
        /// <param name="testItem">Function delegate used to test item <paramref name="value"/>.</param>
        /// <param name="adapter">Adapter reference if found; otherwise null.</param>
        /// <returns><c>true</c> if adapter with the specified <paramref name="value"/> was found; otherwise <c>false</c>.</returns>
        protected virtual bool TryGetAdapter<TValue>(TValue value, Func<T, TValue, bool> testItem, out T adapter)
        {
            lock (this)
            {
                foreach (T item in this)
                {
                    if (testItem(item, value))
                    {
                        adapter = item;
                        return true;
                    }
                }
            }

            adapter = default(T);
            return false;
        }

        // Explicit IAdapter implementation of TryGetAdapterByID
        bool IAdapterCollection.TryGetAdapterByID(uint id, out IAdapter adapter)
        {
            T typedAdapter;
            bool result = TryGetAdapterByID(id, out typedAdapter);
            adapter = typedAdapter;
            return result;
        }

        // Explicit IAdapter implementation of TryGetAdapterByName
        bool IAdapterCollection.TryGetAdapterByName(string name, out IAdapter adapter)
        {
            T typedAdapter;
            bool result = TryGetAdapterByName(name, out typedAdapter);
            adapter = typedAdapter;
            return result;
        }

        /// <summary>
        /// Attempts to initialize (or reinitialize) an individual <see cref="IAdapter"/> based on its ID.
        /// </summary>
        /// <param name="id">The numeric ID associated with the <see cref="IAdapter"/> to be initialized.</param>
        /// <returns><c>true</c> if item was successfully initialized; otherwise <c>false</c>.</returns>
        public virtual bool TryInitializeAdapterByID(uint id)
        {
            T newAdapter, oldAdapter;

            DataRow[] rows;

            if (m_temporalCollection)
                rows = DataSource.Tables[DataMember].Select(string.Format("ID = {0} AND TemporalSession <> 0", id));
            else
                rows = DataSource.Tables[DataMember].Select(string.Format("ID = {0}", id));

            if (rows.Length > 0)
            {
                if (TryCreateAdapter(rows[0], out newAdapter))
                {
                    // Found and created new item - update collection reference
                    bool foundItem = false;

                    lock (this)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            oldAdapter = this[i];

                            if (oldAdapter.ID == id)
                            {
                                // Stop old item
                                oldAdapter.Stop();

                                // Dispose old item, initialize new item
                                this[i] = newAdapter;

                                foundItem = true;
                                break;
                            }
                        }

                        // Add item to collection if it didn't exist
                        if (!foundItem)
                            Add(newAdapter);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Starts each <see cref="IAdapter"/> implementation in this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        /// <remarks>
        /// When starting the adapter collections, the <see cref="DelayAutoStart"/> will set to <c>false</c> so
        /// that adapters can actually be started.
        /// </remarks>
        [AdapterCommand("Starts each adapter in the collection.", "Administrator", "Editor")]
        public virtual void Start()
        {
            // Make sure we are stopped (e.g., disconnected) before attempting to start (e.g., connect)
            if (!m_enabled)
            {
                m_enabled = true;

                // We must set delay auto-start to false so that adapters can start - this can been set
                // during initialization to delay the automatic starting process as needed, but now that
                // start has been requested, the adapters must start.
                DelayAutoStart = false;

                ResetStatistics();

                // This will "restart" all adapters that were previously stopped - note that during startup
                // any adapters that were set to auto-start will be automatically started after initialization
                lock (this)
                {
                    foreach (T item in this)
                    {
                        if (item.Initialized && item.AutoStart && !item.Enabled)
                            item.Start();
                    }
                }

                // Start data monitor...
                if (MonitorTimerEnabled)
                    m_monitorTimer.Start();
            }
        }

        /// <summary>
        /// Stops each <see cref="IAdapter"/> implementation in this <see cref="AdapterCollectionBase{T}"/>.
        /// </summary>
        [AdapterCommand("Stops each adapter in the collection.", "Administrator", "Editor")]
        public virtual void Stop()
        {
            if (m_enabled)
            {
                m_enabled = false;

                lock (this)
                {
                    foreach (T item in this)
                    {
                        if (item.Initialized && item.Enabled)
                            item.Stop();
                    }
                }

                // Stop data monitor...
                m_monitorTimer.Stop();
            }
        }

        /// <summary>
        /// Resets the statistics of this collection.
        /// </summary>
        [AdapterCommand("Resets the statistics of this collection.", "Administrator", "Editor")]
        public void ResetStatistics()
        {
            m_processedEntities = 0;
            m_totalProcessTime = 0.0D;
            m_lastProcessTime = DateTime.UtcNow.Ticks;

            OnStatusMessage("Statistics reset for this collection.");
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="AdapterBase"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of this <see cref="AdapterBase"/>.</returns>
        public virtual string GetShortStatus(int maxLength)
        {
            return string.Format("Total components: {0:N0}", Count).CenterText(maxLength);
        }

        /// <summary>
        /// Defines a temporal processing constraint for the adapter collection and applies this constraint to each adapter.
        /// </summary>
        /// <param name="startTime">Defines a relative or exact start time for the temporal constraint.</param>
        /// <param name="stopTime">Defines a relative or exact stop time for the temporal constraint.</param>
        /// <param name="constraintParameters">Defines any temporal parameters related to the constraint.</param>
        /// <remarks>
        /// <para>
        /// This method defines a temporal processing constraint for an adapter, i.e., the start and stop time over which an
        /// adapter will process data. Actual implementation of the constraint will be adapter specific. Implementations
        /// should be able to dynamically handle multiple calls to this function with new constraints. Passing in <c>null</c>
        /// for the <paramref name="startTime"/> and <paramref name="stopTime"/> should cancel the temporal constraint and
        /// return the adapter to standard / real-time operation.
        /// </para>
        /// <para>
        /// The <paramref name="startTime"/> and <paramref name="stopTime"/> parameters can be specified in one of the
        /// following formats:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Time Format</term>
        ///         <description>Format Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>12-30-2000 23:59:59.033</term>
        ///         <description>Absolute date and time.</description>
        ///     </item>
        ///     <item>
        ///         <term>*</term>
        ///         <description>Evaluates to <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-20s</term>
        ///         <description>Evaluates to 20 seconds before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-10m</term>
        ///         <description>Evaluates to 10 minutes before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-1h</term>
        ///         <description>Evaluates to 1 hour before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>*-1d</term>
        ///         <description>Evaluates to 1 day before <see cref="DateTime.UtcNow"/>.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        [AdapterCommand("Defines a temporal processing constraint for each adapter in the collection.", "Administrator", "Editor", "Viewer")]
        public virtual void SetTemporalConstraint(string startTime, string stopTime, string constraintParameters)
        {
            // Apply temporal constraint to all adapters in this collection
            lock (this)
            {
                foreach (T adapter in this)
                {
                    adapter.SetTemporalConstraint(startTime, stopTime, constraintParameters);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="ProcessException"/> event.
        /// </summary>
        /// <param name="ex">Processing <see cref="Exception"/>.</param>
        // internal protection level specified to allow attachment via temporal sessions
        internal protected virtual void OnProcessException(Exception ex)
        {
            if ((object)ProcessException != null)
                ProcessException(this, new EventArgs<Exception>(ex));
        }

        /// <summary>
        /// Raises the <see cref="StatusMessage"/> event.
        /// </summary>
        /// <param name="status">New status message.</param>
        protected virtual void OnStatusMessage(string status)
        {
            try
            {
                if ((object)StatusMessage != null)
                    StatusMessage(this, new EventArgs<string>(status));
            }
            catch (Exception ex)
            {
                // We protect our code from consumer thrown exceptions
                OnProcessException(new InvalidOperationException(string.Format("Exception in consumer handler for StatusMessage event: {0}", ex.Message), ex));
            }
        }

        /// <summary>
        /// Raises the <see cref="StatusMessage"/> event with a formatted status message.
        /// </summary>
        /// <param name="formattedStatus">Formatted status message.</param>
        /// <param name="args">Arguments for <paramref name="formattedStatus"/>.</param>
        /// <remarks>
        /// This overload combines string.Format and SendStatusMessage for convenience.
        /// </remarks>
        // internal protection level specified to allow attachment via temporal sessions
        internal protected virtual void OnStatusMessage(string formattedStatus, params object[] args)
        {
            try
            {
                if ((object)StatusMessage != null)
                    StatusMessage(this, new EventArgs<string>(string.Format(formattedStatus, args)));
            }
            catch (Exception ex)
            {
                // We protect our code from consumer thrown exceptions
                OnProcessException(new InvalidOperationException(string.Format("Exception in consumer handler for StatusMessage event: {0}", ex.Message), ex));
            }
        }

        // The following Collection<T> overrides allow operational interceptions to collection that we use to automatically manage adapter lifecycles

        /// <summary>
        /// Removes all elements from the <see cref="Collection{T}"/>.
        /// </summary>
        protected override void ClearItems()
        {
            // Dispose each item before clearing the collection
            lock (this)
            {
                foreach (T item in this)
                {
                    DisposeItem(item);
                }

                base.ClearItems();
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="Collection{T}"/> the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The <see cref="IAdapter"/> implementation to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            lock (this)
            {
                // Wire up item events and handle item initialization
                InitializeItem(item);
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Assigns a new element to the <see cref="Collection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index for which item should be assigned.</param>
        /// <param name="item">The <see cref="IAdapter"/> implementation to assign.</param>
        protected override void SetItem(int index, T item)
        {
            lock (this)
            {
                // Dispose of existing item
                DisposeItem(this[index]);

                // Wire up item events and handle initialization of new item
                InitializeItem(item);

                base.SetItem(index, item);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            // Dispose of item before removing it from the collection
            lock (this)
            {
                DisposeItem(this[index]);
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// Wires events and initializes new <see cref="IAdapter"/> implementation.
        /// </summary>
        /// <param name="item">New <see cref="IAdapter"/> implementation.</param>
        /// <remarks>
        /// Derived classes should override if more events are defined.
        /// </remarks>
        protected virtual void InitializeItem(T item)
        {
            if ((object)item != null)
            {
                // Wire up events
                item.StatusMessage += item_StatusMessage;
                item.ProcessException += item_ProcessException;
                item.InputSignalsUpdated += item_InputSignalsUpdated;
                item.OutputSignalsUpdated += item_OutputSignalsUpdated;
                item.ConfigurationChanged += item_ConfigurationChanged;
                item.EntitiesDiscarded += item_EntitiesDiscarded;
                item.Disposed += item_Disposed;

                try
                {
                    // If automatically initializing new elements, handle object initialization from
                    // thread pool so it can take needed amount of time
                    if (AutoInitialize)
                    {
                        Thread itemThread = new Thread(InitializeAndStartItem);
                        itemThread.IsBackground = true;
                        itemThread.Start(item);
                    }
                }
                catch (Exception ex)
                {
                    // Process exception for logging
                    string errorMessage = string.Format("Failed to queue initialize operation for adapter {0}: {1}", item.Name, ex.Message);
                    OnProcessException(new InvalidOperationException(errorMessage, ex));
                }
            }
        }

        /// <summary>
        /// Unwires events and disposes of <see cref="IAdapter"/> implementation.
        /// </summary>
        /// <param name="item"><see cref="IAdapter"/> to dispose.</param>
        /// <remarks>
        /// Derived classes should override if more events are defined.
        /// </remarks>
        protected virtual void DisposeItem(T item)
        {
            if ((object)item != null)
            {
                // Un-wire events
                item.StatusMessage -= item_StatusMessage;
                item.ProcessException -= item_ProcessException;
                item.InputSignalsUpdated -= item_InputSignalsUpdated;
                item.OutputSignalsUpdated -= item_OutputSignalsUpdated;
                item.ConfigurationChanged -= item_ConfigurationChanged;
                item.EntitiesDiscarded -= item_EntitiesDiscarded;

                // Dispose of item, then un-wire disposed event
                item.Dispose();
                item.Disposed -= item_Disposed;
            }
        }

        // Thread delegate to handle item startup
        private void InitializeAndStartItem(object state)
        {
            T item = state as T;

            if ((object)item == null)
                return;

            Timer initializationTimeoutTimer = null;

            try
            {
                // If initialization timeout is specified for this item, start the initialization timeout timer
                if (item.InitializationTimeout > 0)
                {
                    initializationTimeoutTimer = new Timer(item.InitializationTimeout);

                    initializationTimeoutTimer.Elapsed += (sender, args) =>
                    {
                        const string MessageFormat = "WARNING: Initialization of adapter {0} has exceeded" +
                            " its timeout of {1} seconds. The adapter may still initialize, however this" +
                            " may indicate a problem with the adapter. If you consider this to be normal," +
                            " try adjusting the initialization timeout to suppress this message during" +
                            " normal operations.";

                        OnStatusMessage(MessageFormat, item.Name, item.InitializationTimeout / 1000.0D);
                    };

                    initializationTimeoutTimer.AutoReset = false;
                    initializationTimeoutTimer.Start();
                }

                // Initialize the item
                item.Initialize();

                // Initialization successfully completed, so stop the timeout timer
                if ((object)initializationTimeoutTimer != null)
                    initializationTimeoutTimer.Stop();

                // Set item to its final initialized state so that start and stop commands may be issued to the adapter
                item.Initialized = true;

                try
                {
                    // If the item is set to auto-start, start it now
                    if (!m_delayAutoStart && item.AutoStart)
                        item.Start();
                }
                catch (Exception ex)
                {
                    // We report any errors encountered during startup...
                    OnProcessException(new InvalidOperationException(string.Format("Failed to start adapter {0}: {1}", item.Name, ex.Message), ex));
                }
            }
            catch (Exception ex)
            {
                // We report any errors encountered during initialization...
                OnProcessException(new InvalidOperationException(string.Format("Failed to initialize adapter {0}: {1}", item.Name, ex.Message), ex));
            }
            finally
            {
                if ((object)initializationTimeoutTimer != null)
                    initializationTimeoutTimer.Dispose();
            }
        }

        // In these individual item handlers it is important to directly raise events instead
        // of going though the "On<EventName>" proxy event raisers so that the source item
        // can be sent in through then "sender" object. Additionally, each item's event
        // raiser is already set to protect code from consumer thrown exceptions so there is
        // no need to replicate this protection.

        // Raise status message event on behalf of each item in collection
        private void item_StatusMessage(object sender, EventArgs<string> e)
        {
            if ((object)StatusMessage != null)
                StatusMessage(sender, e);
        }

        // Raise process exception event on behalf of each item in collection
        private void item_ProcessException(object sender, EventArgs<Exception> e)
        {
            if ((object)ProcessException != null)
                ProcessException(sender, e);
        }

        // Raise input signals updated event on behalf of each item in collection
        private void item_InputSignalsUpdated(object sender, EventArgs e)
        {
            if ((object)InputSignalsUpdated != null)
                InputSignalsUpdated(sender, e);
        }

        // Raise output signals updated event on behalf of each item in collection
        private void item_OutputSignalsUpdated(object sender, EventArgs e)
        {
            if ((object)OutputSignalsUpdated != null)
                OutputSignalsUpdated(sender, e);
        }

        // Raise configuration changed event on behalf of each item in collection
        private void item_ConfigurationChanged(object sender, EventArgs e)
        {
            if ((object)ConfigurationChanged != null)
                ConfigurationChanged(sender, e);
        }

        // Raise entities discarded event on behalf of each item in collection
        private void item_EntitiesDiscarded(object sender, EventArgs<IEnumerable<ITimeSeriesEntity>> e)
        {
            if ((object)EntitiesDiscarded != null)
                EntitiesDiscarded(sender, e);
        }

        // Raise disposed event on behalf of each item in collection
        private void item_Disposed(object sender, EventArgs e)
        {
            if ((object)Disposed != null)
                Disposed(sender, e);
        }

        // We monitor the total number of entities destined for archival here...
        private void m_monitorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StringBuilder status = new StringBuilder();
            Ticks currentTime, totalProcessTime;
            long totalNew, processedEntities = ProcessedEntities;

            // Calculate time since last call
            currentTime = DateTime.UtcNow.Ticks;
            totalProcessTime = currentTime - m_lastProcessTime;
            m_totalProcessTime += totalProcessTime.ToSeconds();
            m_lastProcessTime = currentTime;

            // Calculate how many new entities have been received in the last minute...
            totalNew = processedEntities - m_processedEntities;
            m_processedEntities = processedEntities;

            // Process statistics for 12 hours total runtime:
            //
            //          1              1                 1
            // 12345678901234 12345678901234567 1234567890
            // Time span          Entities      Per second
            // -------------- ----------------- ----------
            // Entire runtime 9,999,999,999,999 99,999,999
            // Last minute         4,985            83

            status.AppendFormat("\r\nProcess statistics for {0} total runtime:\r\n\r\n", m_totalProcessTime.ToString().ToLower());
            status.Append("Time span".PadRight(14));
            status.Append(' ');
            status.Append("Entities".CenterText(17));
            status.Append(' ');
            status.Append("Per second".CenterText(10));
            status.AppendLine();
            status.Append(new string('-', 14));
            status.Append(' ');
            status.Append(new string('-', 17));
            status.Append(' ');
            status.Append(new string('-', 10));
            status.AppendLine();

            status.Append("Entire runtime".PadRight(14));
            status.Append(' ');
            status.Append(m_processedEntities.ToString("N0").CenterText(17));
            status.Append(' ');
            status.Append(((int)(m_processedEntities / m_totalProcessTime)).ToString("N0").CenterText(10));
            status.AppendLine();
            status.Append("Last minute".PadRight(14));
            status.Append(' ');
            status.Append(totalNew.ToString("N0").CenterText(17));
            status.Append(' ');
            status.Append(((int)(totalNew / totalProcessTime.ToSeconds())).ToString("N0").CenterText(10));

            // Report updated statistics every minute...
            OnStatusMessage(status.ToString());
        }

        #region [ Explicit IList<IAdapter> Implementation ]

        void ICollection<IAdapter>.Add(IAdapter item)
        {
            lock (this)
            {
                Add((T)item);
            }
        }

        bool ICollection<IAdapter>.Contains(IAdapter item)
        {
            lock (this)
            {
                return Contains((T)item);
            }
        }

        void ICollection<IAdapter>.CopyTo(IAdapter[] array, int arrayIndex)
        {
            lock (this)
            {
                CopyTo(array.Cast<T>().ToArray(), arrayIndex);
            }
        }

        bool ICollection<IAdapter>.Remove(IAdapter item)
        {
            lock (this)
            {
                return Remove((T)item);
            }
        }

        IEnumerator<IAdapter> IEnumerable<IAdapter>.GetEnumerator()
        {
            IAdapter[] adapters;

            lock (this)
            {
                adapters = new IAdapter[Count];

                for (int i = 0; i < Count; i++)
                    adapters[i] = this[i];
            }

            return ((IEnumerable<IAdapter>)adapters).GetEnumerator();
        }

        int IList<IAdapter>.IndexOf(IAdapter item)
        {
            lock (this)
            {
                return IndexOf((T)item);
            }
        }

        void IList<IAdapter>.Insert(int index, IAdapter item)
        {
            lock (this)
            {
                Insert(index, (T)item);
            }
        }

        IAdapter IList<IAdapter>.this[int index]
        {
            get
            {
                lock (this)
                {
                    return this[index];
                }
            }
            set
            {
                lock (this)
                {
                    this[index] = (T)value;
                }
            }
        }

        #endregion

        #endregion
    }
}