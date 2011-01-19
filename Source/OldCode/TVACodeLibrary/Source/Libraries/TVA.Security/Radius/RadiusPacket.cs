﻿//*******************************************************************************************************
//  RadiusPacket.cs - Gbtc
//
//  Tennessee Valley Authority, 2010
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  This software is made freely available under the TVA Open Source Agreement (see below).
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  11/26/2010 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

#region [ TVA Open Source Agreement ]
/*

 THIS OPEN SOURCE AGREEMENT ("AGREEMENT") DEFINES THE RIGHTS OF USE,REPRODUCTION, DISTRIBUTION,
 MODIFICATION AND REDISTRIBUTION OF CERTAIN COMPUTER SOFTWARE ORIGINALLY RELEASED BY THE
 TENNESSEE VALLEY AUTHORITY, A CORPORATE AGENCY AND INSTRUMENTALITY OF THE UNITED STATES GOVERNMENT
 ("GOVERNMENT AGENCY"). GOVERNMENT AGENCY IS AN INTENDED THIRD-PARTY BENEFICIARY OF ALL SUBSEQUENT
 DISTRIBUTIONS OR REDISTRIBUTIONS OF THE SUBJECT SOFTWARE. ANYONE WHO USES, REPRODUCES, DISTRIBUTES,
 MODIFIES OR REDISTRIBUTES THE SUBJECT SOFTWARE, AS DEFINED HEREIN, OR ANY PART THEREOF, IS, BY THAT
 ACTION, ACCEPTING IN FULL THE RESPONSIBILITIES AND OBLIGATIONS CONTAINED IN THIS AGREEMENT.

 Original Software Designation: openPDC
 Original Software Title: The TVA Open Source Phasor Data Concentrator
 User Registration Requested. Please Visit https://naspi.tva.com/Registration/
 Point of Contact for Original Software: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>

 1. DEFINITIONS

 A. "Contributor" means Government Agency, as the developer of the Original Software, and any entity
 that makes a Modification.

 B. "Covered Patents" mean patent claims licensable by a Contributor that are necessarily infringed by
 the use or sale of its Modification alone or when combined with the Subject Software.

 C. "Display" means the showing of a copy of the Subject Software, either directly or by means of an
 image, or any other device.

 D. "Distribution" means conveyance or transfer of the Subject Software, regardless of means, to
 another.

 E. "Larger Work" means computer software that combines Subject Software, or portions thereof, with
 software separate from the Subject Software that is not governed by the terms of this Agreement.

 F. "Modification" means any alteration of, including addition to or deletion from, the substance or
 structure of either the Original Software or Subject Software, and includes derivative works, as that
 term is defined in the Copyright Statute, 17 USC § 101. However, the act of including Subject Software
 as part of a Larger Work does not in and of itself constitute a Modification.

 G. "Original Software" means the computer software first released under this Agreement by Government
 Agency entitled openPDC, including source code, object code and accompanying documentation, if any.

 H. "Recipient" means anyone who acquires the Subject Software under this Agreement, including all
 Contributors.

 I. "Redistribution" means Distribution of the Subject Software after a Modification has been made.

 J. "Reproduction" means the making of a counterpart, image or copy of the Subject Software.

 K. "Sale" means the exchange of the Subject Software for money or equivalent value.

 L. "Subject Software" means the Original Software, Modifications, or any respective parts thereof.

 M. "Use" means the application or employment of the Subject Software for any purpose.

 2. GRANT OF RIGHTS

 A. Under Non-Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor,
 with respect to its own contribution to the Subject Software, hereby grants to each Recipient a
 non-exclusive, world-wide, royalty-free license to engage in the following activities pertaining to
 the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Modification

 5. Redistribution

 6. Display

 B. Under Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor, with
 respect to its own contribution to the Subject Software, hereby grants to each Recipient under Covered
 Patents a non-exclusive, world-wide, royalty-free license to engage in the following activities
 pertaining to the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Sale

 5. Offer for Sale

 C. The rights granted under Paragraph B. also apply to the combination of a Contributor's Modification
 and the Subject Software if, at the time the Modification is added by the Contributor, the addition of
 such Modification causes the combination to be covered by the Covered Patents. It does not apply to
 any other combinations that include a Modification. 

 D. The rights granted in Paragraphs A. and B. allow the Recipient to sublicense those same rights.
 Such sublicense must be under the same terms and conditions of this Agreement.

 3. OBLIGATIONS OF RECIPIENT

 A. Distribution or Redistribution of the Subject Software must be made under this Agreement except for
 additions covered under paragraph 3H. 

 1. Whenever a Recipient distributes or redistributes the Subject Software, a copy of this Agreement
 must be included with each copy of the Subject Software; and

 2. If Recipient distributes or redistributes the Subject Software in any form other than source code,
 Recipient must also make the source code freely available, and must provide with each copy of the
 Subject Software information on how to obtain the source code in a reasonable manner on or through a
 medium customarily used for software exchange.

 B. Each Recipient must ensure that the following copyright notice appears prominently in the Subject
 Software:

          No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.

 C. Each Contributor must characterize its alteration of the Subject Software as a Modification and
 must identify itself as the originator of its Modification in a manner that reasonably allows
 subsequent Recipients to identify the originator of the Modification. In fulfillment of these
 requirements, Contributor must include a file (e.g., a change log file) that describes the alterations
 made and the date of the alterations, identifies Contributor as originator of the alterations, and
 consents to characterization of the alterations as a Modification, for example, by including a
 statement that the Modification is derived, directly or indirectly, from Original Software provided by
 Government Agency. Once consent is granted, it may not thereafter be revoked.

 D. A Contributor may add its own copyright notice to the Subject Software. Once a copyright notice has
 been added to the Subject Software, a Recipient may not remove it without the express permission of
 the Contributor who added the notice.

 E. A Recipient may not make any representation in the Subject Software or in any promotional,
 advertising or other material that may be construed as an endorsement by Government Agency or by any
 prior Recipient of any product or service provided by Recipient, or that may seek to obtain commercial
 advantage by the fact of Government Agency's or a prior Recipient's participation in this Agreement.

 F. In an effort to track usage and maintain accurate records of the Subject Software, each Recipient,
 upon receipt of the Subject Software, is requested to register with Government Agency by visiting the
 following website: https://naspi.tva.com/Registration/. Recipient's name and personal information
 shall be used for statistical purposes only. Once a Recipient makes a Modification available, it is
 requested that the Recipient inform Government Agency at the web site provided above how to access the
 Modification.

 G. Each Contributor represents that that its Modification does not violate any existing agreements,
 regulations, statutes or rules, and further that Contributor has sufficient rights to grant the rights
 conveyed by this Agreement.

 H. A Recipient may choose to offer, and to charge a fee for, warranty, support, indemnity and/or
 liability obligations to one or more other Recipients of the Subject Software. A Recipient may do so,
 however, only on its own behalf and not on behalf of Government Agency or any other Recipient. Such a
 Recipient must make it absolutely clear that any such warranty, support, indemnity and/or liability
 obligation is offered by that Recipient alone. Further, such Recipient agrees to indemnify Government
 Agency and every other Recipient for any liability incurred by them as a result of warranty, support,
 indemnity and/or liability offered by such Recipient.

 I. A Recipient may create a Larger Work by combining Subject Software with separate software not
 governed by the terms of this agreement and distribute the Larger Work as a single product. In such
 case, the Recipient must make sure Subject Software, or portions thereof, included in the Larger Work
 is subject to this Agreement.

 J. Notwithstanding any provisions contained herein, Recipient is hereby put on notice that export of
 any goods or technical data from the United States may require some form of export license from the
 U.S. Government. Failure to obtain necessary export licenses may result in criminal liability under
 U.S. laws. Government Agency neither represents that a license shall not be required nor that, if
 required, it shall be issued. Nothing granted herein provides any such export license.

 4. DISCLAIMER OF WARRANTIES AND LIABILITIES; WAIVER AND INDEMNIFICATION

 A. No Warranty: THE SUBJECT SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTY OF ANY KIND, EITHER
 EXPRESSED, IMPLIED, OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, ANY WARRANTY THAT THE SUBJECT
 SOFTWARE WILL CONFORM TO SPECIFICATIONS, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 PARTICULAR PURPOSE, OR FREEDOM FROM INFRINGEMENT, ANY WARRANTY THAT THE SUBJECT SOFTWARE WILL BE ERROR
 FREE, OR ANY WARRANTY THAT DOCUMENTATION, IF PROVIDED, WILL CONFORM TO THE SUBJECT SOFTWARE. THIS
 AGREEMENT DOES NOT, IN ANY MANNER, CONSTITUTE AN ENDORSEMENT BY GOVERNMENT AGENCY OR ANY PRIOR
 RECIPIENT OF ANY RESULTS, RESULTING DESIGNS, HARDWARE, SOFTWARE PRODUCTS OR ANY OTHER APPLICATIONS
 RESULTING FROM USE OF THE SUBJECT SOFTWARE. FURTHER, GOVERNMENT AGENCY DISCLAIMS ALL WARRANTIES AND
 LIABILITIES REGARDING THIRD-PARTY SOFTWARE, IF PRESENT IN THE ORIGINAL SOFTWARE, AND DISTRIBUTES IT
 "AS IS."

 B. Waiver and Indemnity: RECIPIENT AGREES TO WAIVE ANY AND ALL CLAIMS AGAINST GOVERNMENT AGENCY, ITS
 AGENTS, EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT. IF RECIPIENT'S USE
 OF THE SUBJECT SOFTWARE RESULTS IN ANY LIABILITIES, DEMANDS, DAMAGES, EXPENSES OR LOSSES ARISING FROM
 SUCH USE, INCLUDING ANY DAMAGES FROM PRODUCTS BASED ON, OR RESULTING FROM, RECIPIENT'S USE OF THE
 SUBJECT SOFTWARE, RECIPIENT SHALL INDEMNIFY AND HOLD HARMLESS  GOVERNMENT AGENCY, ITS AGENTS,
 EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT, TO THE EXTENT PERMITTED BY
 LAW.  THE FOREGOING RELEASE AND INDEMNIFICATION SHALL APPLY EVEN IF THE LIABILITIES, DEMANDS, DAMAGES,
 EXPENSES OR LOSSES ARE CAUSED, OCCASIONED, OR CONTRIBUTED TO BY THE NEGLIGENCE, SOLE OR CONCURRENT, OF
 GOVERNMENT AGENCY OR ANY PRIOR RECIPIENT.  RECIPIENT'S SOLE REMEDY FOR ANY SUCH MATTER SHALL BE THE
 IMMEDIATE, UNILATERAL TERMINATION OF THIS AGREEMENT.

 5. GENERAL TERMS

 A. Termination: This Agreement and the rights granted hereunder will terminate automatically if a
 Recipient fails to comply with these terms and conditions, and fails to cure such noncompliance within
 thirty (30) days of becoming aware of such noncompliance. Upon termination, a Recipient agrees to
 immediately cease use and distribution of the Subject Software. All sublicenses to the Subject
 Software properly granted by the breaching Recipient shall survive any such termination of this
 Agreement.

 B. Severability: If any provision of this Agreement is invalid or unenforceable under applicable law,
 it shall not affect the validity or enforceability of the remainder of the terms of this Agreement.

 C. Applicable Law: This Agreement shall be subject to United States federal law only for all purposes,
 including, but not limited to, determining the validity of this Agreement, the meaning of its
 provisions and the rights, obligations and remedies of the parties.

 D. Entire Understanding: This Agreement constitutes the entire understanding and agreement of the
 parties relating to release of the Subject Software and may not be superseded, modified or amended
 except by further written agreement duly executed by the parties.

 E. Binding Authority: By accepting and using the Subject Software under this Agreement, a Recipient
 affirms its authority to bind the Recipient to all terms and conditions of this Agreement and that
 Recipient hereby agrees to all terms and conditions herein.

 F. Point of Contact: Any Recipient contact with Government Agency is to be directed to the designated
 representative as follows: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>.

*/
#endregion

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TVA.Parsing;

namespace TVA.Security.Radius
{
    #region [ Enumerations ]

    /// <summary>
    /// Specifies the type of RADIUS packet.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Packet sent to a RADIUS server for verification of credentials.
        /// </summary>
        AccessRequest = 1,
        /// <summary>
        /// Packet sent by a RADIUS server when credential verification is successful.
        /// </summary>
        AccessAccept = 2,
        /// <summary>
        /// Packet sent by a RADIUS server when credential verification is unsuccessful.
        /// </summary>
        AccessReject = 3,
        /// <summary>
        /// Not used. No description available.
        /// </summary>
        AccountingRequest = 4,
        /// <summary>
        /// Not used. No description available.
        /// </summary>
        AccountingResponse = 5,
        /// <summary>
        /// Not used. No description available. [RFC 2882]
        /// </summary>
        AccountingStatus = 6,
        /// <summary>
        /// Not used. No description available. [RFC 2882]
        /// </summary>
        PasswordRequest = 7,
        /// <summary>
        /// Not used. No description available. [RFC 2882]
        /// </summary>
        PasswordAccept = 8,
        /// <summary>
        /// Not used. No description available. [RFC 2882]
        /// </summary>
        PasswordReject = 9,
        /// <summary>
        /// Not used. No description available. [RFC 2882]
        /// </summary>
        AccountingMessage = 10,
        /// <summary>
        /// Packet sent by a RADIUS server when further information is needed for credential verification.
        /// </summary>
        AccessChallenge = 11,
        /// <summary>
        /// Not used. No description available.
        /// </summary>
        StatuServer = 12,
        /// <summary>
        /// Not used. No description available.
        /// </summary>
        StatusClient = 13
    }

    #endregion

    /// <summary>
    /// Represents a data packet transferred between RADIUS client and server.
    /// </summary>
    public class RadiusPacket : ISupportBinaryImage
    {
        // 0                   1                   2                   3
        // 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //|     Code      |  Identifier   |            Length             |
        //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //|                                                               |
        //|                         Authenticator                         |
        //|                                                               |
        //|                                                               |
        //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //|  Attributes ...
        //+-+-+-+-+-+-+-+-+-+-+-+-+-

        #region [ Members ]

        // Fields
        private PacketType m_type;
        private byte m_identifier;
        private byte[] m_authenticator;
        private List<RadiusPacketAttribute> m_attributes;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a default instance of RADIUS packet.
        /// </summary>
        public RadiusPacket()
        {
            m_identifier = (byte)(Cryptography.Random.Between(0, 255));
            m_authenticator = new byte[16];
            m_attributes = new List<RadiusPacketAttribute>();
        }

        /// <summary>
        /// Creates an instance of RADIUS packet.
        /// </summary>
        /// <param name="type">Type of the packet.</param>
        public RadiusPacket(PacketType type)
            : this()
        {
            m_type = type;
        }

        /// <summary>
        /// Creates an instance of RADIUS packet.
        /// </summary>
        /// <param name="binaryImage">A byte array.</param>
        /// <param name="startIndex">Starting point in the byte array.</param>
        public RadiusPacket(byte[] binaryImage, int startIndex)
            : this()
        {
            Initialize(binaryImage, startIndex, binaryImage.Length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the type of the packet.
        /// </summary>
        /// <value></value>
        /// <returns>Type of the packet.</returns>
        public PacketType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet identifier.
        /// </summary>
        /// <value></value>
        /// <returns>Identifier of the packet.</returns>
        public byte Identifier
        {
            get
            {
                return m_identifier;
            }
            set
            {
                m_identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet authenticator.
        /// </summary>
        /// <value></value>
        /// <returns>Authenticator of the packet.</returns>
        public byte[] Authenticator
        {
            get
            {
                return m_authenticator;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length == 16)
                    {
                        m_authenticator = value;
                    }
                    else
                    {
                        throw new ArgumentException("Authenticator must 16-byte long.");
                    }
                }
                else
                {
                    throw new ArgumentNullException("Authenticator");
                }
            }
        }

        /// <summary>
        /// Gets a list of packet attributes.
        /// </summary>
        /// <value></value>
        /// <returns>Attributes of the packet.</returns>
        public List<RadiusPacketAttribute> Attributes
        {
            get
            {
                return m_attributes;
            }
        }

        /// <summary>
        /// Gets the binary lenght of the packet.
        /// </summary>
        /// <value></value>
        /// <returns>32-bit signed integer value.</returns>
        public int BinaryLength
        {
            get
            {
                // 20 bytes are fixed + length of all attributes combined
                int length = 20;
                foreach (RadiusPacketAttribute attribute in m_attributes)
                {
                    length += attribute.BinaryLength;
                }

                return length;
            }
        }

        /// <summary>
        /// Gets the binary image of the packet.
        /// </summary>
        /// <value></value>
        /// <returns>A byte array.</returns>
        public byte[] BinaryImage
        {
            get
            {
                byte[] image = new byte[BinaryLength];
                image[0] = System.Convert.ToByte(m_type);
                image[1] = m_identifier;
                Array.Copy(GetBytes((ushort)BinaryLength), 0, image, 2, 2);
                Array.Copy(m_authenticator, 0, image, 4, m_authenticator.Length);
                int cursor = 20;
                foreach (RadiusPacketAttribute attribute in m_attributes)
                {
                    Array.Copy(attribute.BinaryImage, 0, image, cursor, attribute.BinaryLength);
                    cursor += attribute.BinaryLength;
                }

                return image;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="RadiusPacket"/> from the specified <paramref name="binaryImage"/>.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="RadiusPacket"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="binaryImage"/> for initializing <see cref="RadiusPacket"/>.</returns>
        public int Initialize(byte[] binaryImage, int startIndex, int length)
        {
            if ((binaryImage != null) && binaryImage.Length >= 20)
            {
                // We have a valid buffer to work with.
                UInt16 size;
                m_type = (PacketType)(binaryImage[startIndex]);
                m_identifier = binaryImage[startIndex + 1];
                size = ToUInt16(binaryImage, startIndex + 2);
                Array.Copy(binaryImage, 4, m_authenticator, 0, m_authenticator.Length);
                // Parse any attributes in the packet.
                int cursor = 20;
                while (cursor < size)
                {
                    RadiusPacketAttribute attribute = new RadiusPacketAttribute(binaryImage, startIndex + cursor);
                    m_attributes.Add(attribute);
                    cursor += attribute.BinaryLength;
                }

                return BinaryLength;
            }
            else
            {
                throw new ArgumentException("Buffer is not valid.");
            }
        }

        /// <summary>
        /// Gets the value of the specified attribute if it is present in the packet.
        /// </summary>
        /// <param name="type">Type of the attribute whose value is to be retrieved.</param>
        /// <returns>Attribute value as a byte array if attribute is present; otherwise Nothing.</returns>
        public byte[] GetAttributeValue(AttributeType type)
        {
            foreach (RadiusPacketAttribute attrib in m_attributes)
            {
                // Attribute found, return its value.
                if (attrib.Type == type)
                {
                    return attrib.Value;
                }
            }

            return null; // Attribute is not present in the packet.
        }

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Encoding format for encoding text.
        /// </summary>
        public static Encoding Encoding = Encoding.UTF8;

        // Static Methods

        /// <summary>
        /// Gets bytes for the specified text.
        /// </summary>
        /// <param name="value">Text blob.</param>
        /// <returns>A byte array.</returns>
        public static byte[] GetBytes(string value)
        {
            return Encoding.GetBytes(value);
        }

        /// <summary>
        /// Gets bytes for the specified 16-bit signed integer value.
        /// </summary>
        /// <param name="value">16-bit signed integer value.</param>
        /// <returns>A byte array.</returns>
        public static byte[] GetBytes(short value)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.GetBytes(value);
        }

        /// <summary>
        /// Gets bytes for the specified 16-bit unsigned integer value.
        /// </summary>
        /// <param name="value">16-bit unsigned integer value.</param>
        /// <returns>A byte array.</returns>
        [CLSCompliant(false)]
        public static byte[] GetBytes(UInt16 value)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.GetBytes(value);
        }

        /// <summary>
        /// Gets bytes for the specified 32-bit signed integer value.
        /// </summary>
        /// <param name="value">32-bit signed integer value.</param>
        /// <returns>A byte array.</returns>
        public static byte[] GetBytes(int value)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.GetBytes(value);
        }

        /// <summary>
        /// Gets bytes for the specified 32-bit unsigned integer value.
        /// </summary>
        /// <param name="value">32-bit unsigned integer value.</param>
        /// <returns>A byte array.</returns>
        [CLSCompliant(false)]
        public static byte[] GetBytes(UInt32 value)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.GetBytes(value);
        }

        /// <summary>
        /// Converts the specified byte array to text.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// <param name="index">Starting point in the byte array.</param>
        /// <param name="length">Number of bytes to be converted.</param>
        /// <returns>A text blob.</returns>
        public static string ToText(byte[] buffer, int index, int length)
        {
            return Encoding.GetString(buffer, index, length);
        }

        /// <summary>
        /// Converts the specified byte array to a signed 16-bit integer value.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// <param name="index">Starting point in the byte array.</param>
        /// <returns>A 16-bit signed integer value.</returns>
        public static short ToInt16(byte[] buffer, int index)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.ToInt16(buffer, index);
        }

        /// <summary>
        /// Converts the specified byte array to an unsigned 16-bit integer value.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// <param name="index">Starting point in the byte array.</param>
        /// <returns>A 16-bit unsigned integer value.</returns>
        [CLSCompliant(false)]
        public static UInt16 ToUInt16(byte[] buffer, int index)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.ToUInt16(buffer, index);
        }

        /// <summary>
        /// Converts the specified byte array to a signed 32-bit integer value.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// <param name="index">Starting point in the byte array.</param>
        /// <returns>A 32-bit signed integer value.</returns>
        public static int ToInt32(byte[] buffer, int index)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.ToInt32(buffer, index);
        }

        /// <summary>
        /// Converts the specified byte array to an unsigned 32-bit integer value.
        /// </summary>
        /// <param name="buffer">A byte array.</param>
        /// <param name="index">Starting point in the byte array.</param>
        /// <returns>A 32-bit unsigned integer value.</returns>
        [CLSCompliant(false)]
        public static UInt32 ToUInt32(byte[] buffer, int index)
        {
            // Integer values are in Big-endian (most significant byte first) format.
            return EndianOrder.BigEndian.ToUInt32(buffer, index);
        }

        /// <summary>
        /// Generates an "Authenticator" value used in a RADIUS request packet sent by the client to server.
        /// </summary>
        /// <param name="sharedSecret">The shared secret to be used in generating the output.</param>
        /// <returns>A byte array.</returns>
        public static byte[] CreateRequestAuthenticator(string sharedSecret)
        {
            // We create a input buffer that'll be used to create a 16-byte value using the RSA MD5 algorithm.
            // Since the output value (The Authenticator) has to be unique over the life of the shared secret,
            // we prepend a randomly generated "salt" text to ensure the uniqueness of the output value.
            byte[] randomBuffer = new byte[16];
            byte[] secretBuffer = RadiusPacket.GetBytes(sharedSecret);
            Cryptography.Random.GetBytes(randomBuffer);

            return new MD5CryptoServiceProvider().ComputeHash(randomBuffer.Combine(secretBuffer));
        }

        /// <summary>
        /// Generates an "Authenticator" value used in a RADIUS response packet sent by the server to client.
        /// </summary>
        /// <param name="sharedSecret">The shared secret key.</param>
        /// <param name="requestPacket">RADIUS packet sent from client to server.</param>
        /// <param name="responsePacket">RADIUS packet sent from server to client.</param>
        /// <returns>A byte array.</returns>
        public static byte[] CreateResponseAuthenticator(string sharedSecret, RadiusPacket requestPacket, RadiusPacket responsePacket)
        {
            byte[] requestBytes = requestPacket.BinaryImage;
            byte[] responseBytes = responsePacket.BinaryImage;
            byte[] sharedSecretBytes = RadiusPacket.GetBytes(sharedSecret);
            byte[] inputBuffer = new byte[responseBytes.Length + sharedSecretBytes.Length];

            // Response authenticator is generated as follows:
            // MD5(Code + Identifier + Length + Request Authenticator + Attributes + Shared Secret)
            //   where:
            //   Code, Identifier, Length & Attributes are from the response RADIUS packet
            //   Request Authenticator if from the request RADIUS packet
            //   Shared Secret is the shared secret ket

            Array.Copy(responseBytes, 0, inputBuffer, 0, responseBytes.Length);
            Array.Copy(requestBytes, 4, inputBuffer, 4, 16);
            Array.Copy(sharedSecretBytes, 0, inputBuffer, responseBytes.Length, sharedSecretBytes.Length);

            return new MD5CryptoServiceProvider().ComputeHash(inputBuffer);
        }

        /// <summary>
        /// Generates an encrypted password using the RADIUS protocol specification (RFC 2285).
        /// </summary>
        /// <param name="password">User's password.</param>
        /// <param name="sharedSecret">Shared secret key.</param>
        /// <param name="requestAuthenticator">Request authenticator byte array.</param>
        /// <returns>A byte array.</returns>
        public static byte[] EncryptPassword(string password, string sharedSecret, byte[] requestAuthenticator)
        {
            // Max length of the password can be 130 according to RFC 2865. Since 128 is the closest multiple
            // of 16 (password segment length), we allow the password to be no longer than 128 characters.
            if (password.Length <= 128)
            {
                byte[] result;
                byte[] xorBytes = null;
                byte[] passwordBytes = RadiusPacket.GetBytes(password);
                byte[] sharedSecretBytes = RadiusPacket.GetBytes(sharedSecret);
                byte[] md5HashInputBytes = new byte[sharedSecretBytes.Length + 16];
                MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
                if (passwordBytes.Length % 16 == 0)
                {
                    // Length of password is a multiple of 16.
                    result = new byte[passwordBytes.Length];
                }
                else
                {
                    // Length of password is not a multiple of 16, so we'll take the multiple of 16 that's next
                    // closest to the password's length and leave the empty space at the end as padding.
                    result = new byte[((passwordBytes.Length / 16) * 16) + 16];
                }

                // Copy the password to the result buffer where it'll be XORed.
                Array.Copy(passwordBytes, 0, result, 0, passwordBytes.Length);
                // For the first 16-byte segment of the password, password characters are to be XORed with the
                // MD5 hash value that's computed as follows:
                //   MD5(Shared secret key + Request authenticator)
                Array.Copy(sharedSecretBytes, 0, md5HashInputBytes, 0, sharedSecretBytes.Length);
                Array.Copy(requestAuthenticator, 0, md5HashInputBytes, sharedSecretBytes.Length, requestAuthenticator.Length);
                for (int i = 0; i <= result.Length - 1; i += 16)
                {
                    // Perform XOR-based encryption of the password in 16-byte segments.
                    if (i > 0)
                    {
                        // For passwords that are more than 16 characters in length, each consecutive 16-byte
                        // segment of the password is XORed with MD5 hash value that's computed as follows:
                        //   MD5(Shared secret key + XOR bytes used in the previous segment)
                        Array.Copy(xorBytes, 0, md5HashInputBytes, sharedSecretBytes.Length, xorBytes.Length);
                    }
                    xorBytes = md5Provider.ComputeHash(md5HashInputBytes);

                    // XOR the password bytes in the current segment with the XOR bytes.
                    for (int j = i; j <= (i + 16) - 1; j++)
                    {
                        result[j] = (byte)(result[j] ^ xorBytes[j]);
                    }
                }

                return result;
            }
            else
            {
                throw new ArgumentException("Password can be a maximum of 128 characters in length.");
            }
        }

        #endregion
    }
}