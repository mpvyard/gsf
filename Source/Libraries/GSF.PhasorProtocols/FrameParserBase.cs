﻿//******************************************************************************************************
//  FrameParserBase.cs - Gbtc
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
//  02/12/2007 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/16/2008 - J. Ritchie Carroll
//       Converted class to inherit from FrameImageParserBase.
//  08/07/2009 - Josh L. Patterson
//       Edited Comments.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  05/25/2012 - J. Ritchie Carroll
//       Fixed an issue with publication of the frame buffer image so that when connection tester
//       serializes these frames they will not overlap when parsing large data sets.
//  12/17/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Text;
using GSF.Collections;
using GSF.Parsing;

namespace GSF.PhasorProtocols
{
    /// <summary>
    /// Represents a frame parser that defines the basic functionality for a protocol to parse a binary data stream and return the parsed data via events.
    /// </summary>
    /// <remarks>
    /// Frame parsers are implemented as a write-only streams so that data can come from any source.<br/>
    /// See <see cref="MultiSourceFrameImageParserBase{TSourceIdentifier,TTypeIdentifier,TOutputType}"/> for more detail.
    /// Note that using the multi-source frame image parser base allows buffers to come in from different channels and still be
    /// parsed correctly.
    /// </remarks>
    /// <typeparam name="TFrameIdentifier">Frame type identifier used to distinguish frames.</typeparam>
    public abstract class FrameParserBase<TFrameIdentifier> : MultiSourceFrameImageParserBase<SourceChannel, TFrameIdentifier, ISupportSourceIdentifiableFrameImage<SourceChannel, TFrameIdentifier>>, IFrameParser
    {
        #region [ Members ]

        // Events

        // Derived classes will typically also expose events to provide instances to the protocol specific final derived channel frames

        /// <summary>
        /// Occurs when a <see cref="IConfigurationFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IConfigurationFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IConfigurationFrame>> ReceivedConfigurationFrame;

        /// <summary>
        /// Occurs when a <see cref="IDataFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IDataFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IDataFrame>> ReceivedDataFrame;

        /// <summary>
        /// Occurs when a <see cref="IHeaderFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="IHeaderFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IHeaderFrame>> ReceivedHeaderFrame;

        /// <summary>
        /// Occurs when a <see cref="ICommandFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="ICommandFrame"/> that was received.
        /// <para>
        /// Command frames are normally sent, not received, but there is nothing that prevents this.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs<ICommandFrame>> ReceivedCommandFrame;

        /// <summary>
        /// Occurs when an undetermined <see cref="IChannelFrame"/> has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the undetermined <see cref="IChannelFrame"/> that was received.
        /// </remarks>
        public event EventHandler<EventArgs<IChannelFrame>> ReceivedUndeterminedFrame;

        /// <summary>
        /// Occurs when a frame image has been received.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T1,T2}.Argument1"/> is the <see cref="FundamentalFrameType"/> of the frame buffer image that was received.<br/>
        /// <see cref="EventArgs{T1,T2}.Argument2"/> is the length of the frame image that was received.
        /// </remarks>
        public event EventHandler<EventArgs<FundamentalFrameType, int>> ReceivedFrameImage;

        /// <summary>
        /// Occurs when a frame image has been received, event includes buffer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument1"/> is the <see cref="FundamentalFrameType"/> of the frame buffer image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument2"/> is the buffer that contains the frame image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument3"/> is the offset into the buffer that contains the frame image that was received.<br/>
        /// <see cref="EventArgs{T1,T2,T3,T4}.Argument4"/> is the length of data in the buffer that contains the frame image that was received.
        /// </para>
        /// <para>
        /// Consumers should use the more efficient <see cref="ReceivedFrameImage"/> event if the buffer is not needed.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs<FundamentalFrameType, byte[], int, int>> ReceivedFrameBufferImage;

        /// <summary>
        /// Occurs when a device sends a notification that its configuration has changed.
        /// </summary>
        public event EventHandler ConfigurationChanged;

        // Fields
        private IConnectionParameters m_connectionParameters;
        private AsyncQueue<EventArgs<FundamentalFrameType, byte[], int, int>> m_frameImageQueue;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="FrameParserBase{TypeIndentifier}"/>.
        /// </summary>
        protected FrameParserBase()
        {
            // We attach to base class DataParsed event to automatically redirect and cast channel frames to their specific output events
            base.DataParsed += base_DataParsed;
            base.DuplicateTypeHandlerEncountered += base_DuplicateTypeHandlerEncountered;
            base.OutputTypeNotFound += base_OutputTypeNotFound;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of redundant frames in each packet.
        /// </summary>
        /// <remarks>
        /// <para>This value is used when calculating statistics. It is assumed that for each
        /// frame that is received, that frame will be included in the next <c>n</c>
        /// packets, where <c>n</c> is the number of redundant frames per packet.</para>
        /// 
        /// <para>This base class returns 0, as most protocols do not support redundant frames.</para>
        /// </remarks>
        public virtual int RedundantFramesPerPacket
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets current <see cref="IConfigurationFrame"/> used for parsing <see cref="IDataFrame"/>'s encountered in the data stream from a device.
        /// </summary>
        /// <remarks>
        /// If a <see cref="IConfigurationFrame"/> has been parsed, this will return a reference to the parsed frame.  Consumer can manually assign a
        /// <see cref="IConfigurationFrame"/> to start parsing data if one has not been encountered in the stream.
        /// </remarks>
        public abstract IConfigurationFrame ConfigurationFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any connection specific <see cref="IConnectionParameters"/> that may be needed for parsing.
        /// </summary>
        public virtual IConnectionParameters ConnectionParameters
        {
            get
            {
                return m_connectionParameters;
            }
            set
            {
                m_connectionParameters = value;
            }
        }

        /// <summary>
        /// Gets current descriptive status of the <see cref="FrameParserBase{TypeIndentifier}"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);
                status.Append("     Received config frame: ");
                status.Append((object)ConfigurationFrame == null ? "No" : "Yes");
                status.AppendLine();

                if ((object)ConfigurationFrame != null)
                {
                    status.Append("   Devices in config frame: ");
                    status.Append(ConfigurationFrame.Cells.Count);
                    status.Append(" total - ");
                    status.AppendLine();

                    foreach (IConfigurationCell cell in ConfigurationFrame.Cells)
                    {
                        status.AppendFormat("               ({0:00000}) {1}{2}\r\n", cell.IDCode, cell.StationName.PadRight(16), string.IsNullOrEmpty(cell.IDLabel) ? "" : string.Format(" [{0}]", cell.IDLabel));
                    }

                    status.Append("     Configured frame rate: ");
                    status.Append(ConfigurationFrame.FrameRate);
                    status.AppendLine();
                }

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FrameParserBase{TypeIndentifier}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        // Detach from base class events
                        base.DataParsed -= base_DataParsed;
                        base.DuplicateTypeHandlerEncountered -= base_DuplicateTypeHandlerEncountered;
                        base.OutputTypeNotFound -= base_OutputTypeNotFound;
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ReceivedConfigurationFrame"/> event.
        /// </summary>
        /// <param name="frame"><see cref="IConfigurationFrame"/> to send to <see cref="ReceivedConfigurationFrame"/> event.</param>
        protected virtual void OnReceivedConfigurationFrame(IConfigurationFrame frame)
        {
            if ((object)ReceivedConfigurationFrame != null)
                ReceivedConfigurationFrame(this, new EventArgs<IConfigurationFrame>(frame));
        }

        /// <summary>
        /// Raises the <see cref="ReceivedDataFrame"/> event.
        /// </summary>
        /// <param name="frame"><see cref="IDataFrame"/> to send to <see cref="ReceivedDataFrame"/> event.</param>
        protected virtual void OnReceivedDataFrame(IDataFrame frame)
        {
            if ((object)ReceivedDataFrame != null)
                ReceivedDataFrame(this, new EventArgs<IDataFrame>(frame));
        }

        /// <summary>
        /// Raises the <see cref="ReceivedHeaderFrame"/> event.
        /// </summary>
        /// <param name="frame"><see cref="IHeaderFrame"/> to send to <see cref="ReceivedHeaderFrame"/> event.</param>
        protected virtual void OnReceivedHeaderFrame(IHeaderFrame frame)
        {
            if ((object)ReceivedHeaderFrame != null)
                ReceivedHeaderFrame(this, new EventArgs<IHeaderFrame>(frame));
        }

        /// <summary>
        /// Raises the <see cref="ReceivedCommandFrame"/> event.
        /// </summary>
        /// <param name="frame"><see cref="ICommandFrame"/> to send to <see cref="ReceivedCommandFrame"/> event.</param>
        protected virtual void OnReceivedCommandFrame(ICommandFrame frame)
        {
            if ((object)ReceivedCommandFrame != null)
                ReceivedCommandFrame(this, new EventArgs<ICommandFrame>(frame));
        }

        /// <summary>
        /// Raises the <see cref="ReceivedUndeterminedFrame"/> event.
        /// </summary>
        /// <param name="frame"><see cref="IChannelFrame"/> to send to <see cref="ReceivedUndeterminedFrame"/> event.</param>
        protected virtual void OnReceivedUndeterminedFrame(IChannelFrame frame)
        {
            if ((object)ReceivedUndeterminedFrame != null)
                ReceivedUndeterminedFrame(this, new EventArgs<IChannelFrame>(frame));
        }

        /// <summary>
        /// Raises the <see cref="ReceivedFrameImage"/> and <see cref="ReceivedFrameBufferImage"/> event.
        /// </summary>
        /// <param name="frameType"><see cref="FundamentalFrameType"/> to send to <see cref="ReceivedFrameBufferImage"/> event.</param>
        /// <param name="buffer">Frame buffer image to send to <see cref="ReceivedFrameBufferImage"/> event.</param>
        /// <param name="offset">Offset into frame buffer image to send to <see cref="ReceivedFrameBufferImage"/> event.</param>
        /// <param name="length">Length of data in frame buffer image to send to <see cref="ReceivedFrameBufferImage"/> event.</param>
        protected virtual void OnReceivedFrameBufferImage(FundamentalFrameType frameType, byte[] buffer, int offset, int length)
        {
            // It is more light-weight for consumers to attach to the "ReceivedFrameImage" event if they don't need the buffer
            if ((object)ReceivedFrameImage != null)
                ReceivedFrameImage(this, new EventArgs<FundamentalFrameType, int>(frameType, length));

            // Since this event is called from an async socket operation, these events can be processed simultaneously, especially
            // when the consuming event may take time to process this data (e.g., writing the frame to a capture file for replay),
            // so we queue these events up for asynchronous serial processing
            if ((object)ReceivedFrameBufferImage != null)
            {
                if ((object)m_frameImageQueue == null)
                {
                    m_frameImageQueue = new AsyncQueue<EventArgs<FundamentalFrameType, byte[], int, int>>
                    {
                        ProcessItemFunction = frameImage => ReceivedFrameBufferImage(this, frameImage)
                    };

                    m_frameImageQueue.ProcessException += (sender, e) => OnParsingException(e.Argument);
                }

                // We don't own the provided buffer and don't know what the consumer will do with it, so we create
                // a copy of the relevant buffer segment and enqueue this for processing
                byte[] bufferSegment = buffer.BlockCopy(offset, length);

                m_frameImageQueue.Enqueue(new EventArgs<FundamentalFrameType, byte[], int, int>(frameType, bufferSegment, 0, bufferSegment.Length));
            }
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event.
        /// </summary>
        protected virtual void OnConfigurationChanged()
        {
            if ((object)ConfigurationChanged != null)
                ConfigurationChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Casts the parsed <see cref="IChannelFrame"/> to its specific implementation (i.e., <see cref="IDataFrame"/>, <see cref="IConfigurationFrame"/>, <see cref="ICommandFrame"/> or <see cref="IHeaderFrame"/>).
        /// </summary>
        /// <param name="frame"><see cref="IChannelFrame"/> that was parsed by <see cref="FrameImageParserBase{TTypeIdentifier,TOutputType}"/> that implements protocol specific common frame header interface.</param>
        protected virtual void OnReceivedChannelFrame(IChannelFrame frame)
        {
            if ((object)frame != null)
            {
                IDataFrame dataFrame = frame as IDataFrame;

                if ((object)dataFrame != null)
                {
                    // Frame was a data frame
                    OnReceivedDataFrame(dataFrame);
                }
                else
                {
                    IConfigurationFrame configFrame = frame as IConfigurationFrame;

                    if ((object)configFrame != null)
                    {
                        // Frame was a configuration frame
                        OnReceivedConfigurationFrame(configFrame);
                    }
                    else
                    {
                        IHeaderFrame headerFrame = frame as IHeaderFrame;

                        if ((object)headerFrame != null)
                        {
                            // Frame was a header frame
                            OnReceivedHeaderFrame(headerFrame);
                        }
                        else
                        {
                            ICommandFrame commandFrame = frame as ICommandFrame;

                            if ((object)commandFrame != null)
                            {
                                // Frame was a command frame
                                OnReceivedCommandFrame(commandFrame);
                            }
                            else
                            {
                                // Frame type was undetermined
                                OnReceivedUndeterminedFrame(frame);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles unknown frame types.
        /// </summary>
        /// <param name="frameType">Unknown frame ID.</param>
        protected virtual void OnUnknownFrameTypeEncountered(TFrameIdentifier frameType)
        {
            OnParsingException(new InvalidOperationException(string.Format("WARNING: Encountered an undefined frame type identfier \"{0}\". Output was not parsed.", frameType)));
        }

        // Handle reception of data from base class event "DataParsed". Note that by attaching to base class event instead of overriding
        // OnDataParsed event raiser we allow frame implementations to control whether or not publication happens from a new threadpool
        // thread or from existing parsing thread by simply overriding the AllowQueuedPublication boolean property. Normally configuration
        // frames are published as soon as they are parsed to make sure needed parsing information is available as quickly as possible.
        // All other frames are queued for processing by default to allow for better processor distribution of mapping/routing work load.
        private void base_DataParsed(object sender, EventArgs<ISupportSourceIdentifiableFrameImage<SourceChannel, TFrameIdentifier>> e)
        {
            // Call overridable channel frame function handler for parsed frame...
            OnReceivedChannelFrame(e.Argument as IChannelFrame);
        }

        // Handles output type not found error from base class event "OutputTypeNotFound"
        private void base_OutputTypeNotFound(object sender, EventArgs<TFrameIdentifier> e)
        {
            // Call overridable output type not found function handler...
            OnUnknownFrameTypeEncountered(e.Argument);
        }

        // Handles duplicate type handler encountered warning from base class event "DuplicateTypeHandlerEncountered"
        private void base_DuplicateTypeHandlerEncountered(object sender, EventArgs<Type, TFrameIdentifier> e)
        {
            // This exception will only occur on start up and is a result of not defining unique frame identifiers for the base types
            OnParsingException(new InvalidOperationException(string.Format("WARNING: Duplicate frame type identfier \"{0}\" encountered for parsing type {1} during initialization. Only the first defined type for this identifier will ever be parsed.", e.Argument2, e.Argument1.FullName)));
        }

        #endregion
    }
}