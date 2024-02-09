namespace CarAuto.Kafka
{
    /// <summary>
    /// Defines the Default Message Headers.
    /// </summary>
    public static class MessageHeaders
    {
        /// <summary>
        /// The RoleId header.
        /// </summary>
        public static string RoleId => "RoleId";

        /// <summary>
        /// The UserId header.
        /// </summary>
        public static string UserId => "UserId";

        /// <summary>
        /// The TenantId header.
        /// </summary>
        public static string TenantId => "TenantId";

        /// <summary>
        /// The CorrelationId header.
        /// </summary>
        public static string CorrelationId => "CorrelationId";
    }
}