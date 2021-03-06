/* 
 * nMQTT, a .Net MQTT v3 client implementation.
 * http://wiki.github.com/markallanson/nmqtt
 * 
 * Copyright (c) 2009 Mark Allanson (mark@markallanson.net) & Contributors
 *
 * Licensed under the MIT License. You may not use this file except 
 * in compliance with the License. You may obtain a copy of the License at
 *
 *     http://www.opensource.org/licenses/mit-license.php
*/

using System.IO;
using System.Text;

namespace Nmqtt
{
    /// <summary>
    ///     Implementation of an MQTT Publish Message, used for publishing telemetry data along a live MQTT stream.
    /// </summary>
    internal sealed partial class MqttPublishMessage : MqttMessage
    {
        /// <summary>
        ///     Gets or sets the variable header contents. Contains extended metadata about the message
        /// </summary>
        /// <value>The variable header.</value>
        public MqttPublishVariableHeader VariableHeader { get; set; }

        /// <summary>
        ///     Gets or sets the payload of the Mqtt Message.
        /// </summary>
        /// <value>The payload of the Mqtt Message.</value>
        public MqttPublishPayload Payload { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttPublishMessage" /> class.
        /// </summary>
        /// <remarks>
        ///     Only called via the MqttMessage.Create operation during processing of an Mqtt message stream.
        /// </remarks>
        public MqttPublishMessage() {
            this.Header = new MqttHeader().AsType(MqttMessageType.Publish);
            this.VariableHeader = new MqttPublishVariableHeader(this.Header);
            this.Payload = new MqttPublishPayload();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttConnectMessage" /> class.
        /// </summary>
        /// <param name="header">The header to use for the message.</param>
        /// <param name="messageStream">The message stream positioned after the header.</param>
        internal MqttPublishMessage(MqttHeader header, Stream messageStream) {
            this.Header = header;
            ReadFrom(messageStream);
        }

        /// <summary>
        ///     Reads a message from the supplied stream.
        /// </summary>
        /// <param name="messageStream">The message stream.</param>
        public override void ReadFrom(Stream messageStream) {
            base.ReadFrom(messageStream);
            this.VariableHeader = new MqttPublishVariableHeader(this.Header, messageStream);
            this.Payload = new MqttPublishPayload(this.Header, this.VariableHeader, messageStream);
        }

        /// <summary>
        ///     ss the message to the supplied stream.
        /// </summary>
        /// <param name="messageStream">The stream to write the message to.</param>
        public override void WriteTo(Stream messageStream) {
            this.Header.WriteTo(VariableHeader.GetWriteLength() + Payload.GetWriteLength(), messageStream);
            this.VariableHeader.WriteTo(messageStream);
            this.Payload.WriteTo(messageStream);
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            sb.Append(base.ToString());
            sb.AppendLine(VariableHeader.ToString());
            sb.AppendLine(Payload.ToString());

            return sb.ToString();
        }
    }
}