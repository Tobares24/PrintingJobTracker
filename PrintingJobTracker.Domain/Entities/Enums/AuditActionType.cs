namespace PrintingJobTracker.Domain.Entities.Enums
{
    public readonly record struct AuditActionType(string Value)
    {
        public static readonly AuditActionType Created = new("CREADO");
        public static readonly AuditActionType Updated = new("ACTUALIZADO");
        public static readonly AuditActionType Deleted = new("ELIMINADO");
        public static readonly AuditActionType Login = new("LOGIN");
    }
}
