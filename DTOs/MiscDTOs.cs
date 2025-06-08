namespace nexus_response.DTOs
{
    /// <summary>
    /// DTO para estatísticas do dashboard
    /// </summary>
    public class DashboardStatsDTO
    {
        public int TotalIncidents { get; set; }

        public int CriticalIncidents { get; set; }

        public int ActiveDevices { get; set; }

        public DateTime LastUpdate { get; set; }

        public double? LatestTemperature { get; set; }

        public double? LatestHumidity { get; set; }

        public double? LatestDistance { get; set; }

        public AccelerometerDTO LatestAccelerometer { get; set; }
    }

    public class IoTDataDetailDTO
    {
        public double? Temperature_C { get; set; }
        public double? Humidity_Percent { get; set; }
        public double? Distance_CM { get; set; }
        public AccelerometerDTO Accelerometer { get; set; }
    }

    public class AccelerometerDTO
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }

    public class IoTDataDTO
    {
        public int DeviceId { get; set; }
        public string Type { get; set; }
        public IoTDataDetailDTO Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class IncidentHistoryDTO
    {
        public Models.Incident Incident { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public List<IoTDataDTO> IoTData { get; set; }
    }

    public class UrgencyClassificationDTO
    {
        public string Level { get; set; }
        public int Score { get; set; }
        public string Factors { get; set; }
    }

    public class CreateIncidentDTO
    {
        public string Description { get; set; }
        public LocationDTO Location { get; set; }
        public string ReportedBy { get; set; }
        public string StatusReport { get; set; }
        public string Source { get; set; }
    }

    public class LocationDTO
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CreateCommentDTO
    {
        public string Content { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class DeviceStatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime LastCommunication { get; set; }
    }
    public class CreateDeviceDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }

    public class UpdateDeviceStatusDTO
    {
        public string Status { get; set; }
    }
}

