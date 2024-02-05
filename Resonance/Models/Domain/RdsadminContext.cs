using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResoClassAPI.Models.Domain;

public partial class RdsadminContext : DbContext
{
    public RdsadminContext()
    {
    }

    public RdsadminContext(DbContextOptions<RdsadminContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CreatedbActivity> CreatedbActivities { get; set; }

    public virtual DbSet<DbAwaitingSnapshot> DbAwaitingSnapshots { get; set; }

    public virtual DbSet<DbBackup> DbBackups { get; set; }

    public virtual DbSet<DbMapping> DbMappings { get; set; }

    public virtual DbSet<DbOfflineAtSnapshot> DbOfflineAtSnapshots { get; set; }

    public virtual DbSet<DbStatusAtSnapshot> DbStatusAtSnapshots { get; set; }

    public virtual DbSet<DoneInitialDbBackup> DoneInitialDbBackups { get; set; }

    public virtual DbSet<DropdbActivity> DropdbActivities { get; set; }

    public virtual DbSet<LogBackupManifest> LogBackupManifests { get; set; }

    public virtual DbSet<LoginModification> LoginModifications { get; set; }

    public virtual DbSet<LoginsPendingInitialPasswordChange> LoginsPendingInitialPasswordChanges { get; set; }

    public virtual DbSet<ModifydbnameActivity> ModifydbnameActivities { get; set; }

    public virtual DbSet<RdsConfiguration> RdsConfigurations { get; set; }

    public virtual DbSet<RdsCustomerTask> RdsCustomerTasks { get; set; }

    public virtual DbSet<RdsOptionInfo> RdsOptionInfos { get; set; }

    public virtual DbSet<RdsOptionSettingsInfo> RdsOptionSettingsInfos { get; set; }

    public virtual DbSet<RdsReadreplicaLag> RdsReadreplicaLags { get; set; }

    public virtual DbSet<RdsSystemDatabaseObjectSyncInfo> RdsSystemDatabaseObjectSyncInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=SqlConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("guest");

        modelBuilder.Entity<CreatedbActivity>(entity =>
        {
            entity.HasKey(e => e.FamilyGuid).HasName("PK__createdb__C30AF25A59ECF549");

            entity.ToTable("createdb_activity", "dbo");

            entity.Property(e => e.FamilyGuid)
                .ValueGeneratedNever()
                .HasColumnName("family_guid");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
            entity.Property(e => e.ProperlyCreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("properly_created_time");
        });

        modelBuilder.Entity<DbAwaitingSnapshot>(entity =>
        {
            entity.HasKey(e => new { e.FamilyGuid, e.RootCause }).HasName("PK__db_await__6DA542F7BDFFED19");

            entity.ToTable("db_awaiting_snapshot", "dbo");

            entity.Property(e => e.FamilyGuid).HasColumnName("family_guid");
            entity.Property(e => e.RootCause)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("root_cause");
        });

        modelBuilder.Entity<DbBackup>(entity =>
        {
            entity.HasKey(e => new { e.FamilyGuid, e.BeforeBackupEpoch }).HasName("PK__db_backu__777E3F06DA97E3F5");

            entity.ToTable("db_backups", "dbo");

            entity.HasIndex(e => new { e.FamilyGuid, e.AfterBackupEpoch }, "family_guid_after_backup_epoch_index");

            entity.Property(e => e.FamilyGuid).HasColumnName("family_guid");
            entity.Property(e => e.BeforeBackupEpoch).HasColumnName("before_backup_epoch");
            entity.Property(e => e.AfterBackupEpoch).HasColumnName("after_backup_epoch");
            entity.Property(e => e.DatabaseBackup).HasColumnName("database_backup");
            entity.Property(e => e.DatabaseBackupS3BucketName)
                .HasMaxLength(2048)
                .HasColumnName("database_backup_s3_bucket_name");
            entity.Property(e => e.DatabaseBackupS3Key)
                .HasMaxLength(2048)
                .HasColumnName("database_backup_s3_key");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
            entity.Property(e => e.OptionalChecksum).HasColumnName("optional_checksum");
        });

        modelBuilder.Entity<DbMapping>(entity =>
        {
            entity.HasKey(e => e.FamilyGuid).HasName("PK__db_mappi__C30AF25AC811DC4E");

            entity.ToTable("db_mappings", "dbo");

            entity.HasIndex(e => e.DatabaseName, "uq_database_name").IsUnique();

            entity.Property(e => e.FamilyGuid)
                .ValueGeneratedNever()
                .HasColumnName("family_guid");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
        });

        modelBuilder.Entity<DbOfflineAtSnapshot>(entity =>
        {
            entity.HasKey(e => e.FamilyGuid).HasName("PK__db_offli__C30AF25A9C7EEAD6");

            entity.ToTable("db_offline_at_snapshot", "dbo");

            entity.Property(e => e.FamilyGuid)
                .ValueGeneratedNever()
                .HasColumnName("family_guid");
        });

        modelBuilder.Entity<DbStatusAtSnapshot>(entity =>
        {
            entity.HasKey(e => new { e.RdsDbUniqueId, e.DbStatus }).HasName("pk_status_at_snapshot");

            entity.ToTable("db_status_at_snapshot", "dbo");

            entity.Property(e => e.RdsDbUniqueId).HasColumnName("rds_db_unique_id");
            entity.Property(e => e.DbStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("db_status");
        });

        modelBuilder.Entity<DoneInitialDbBackup>(entity =>
        {
            entity.HasKey(e => e.FamilyGuid).HasName("PK__done_ini__C30AF25A4534A075");

            entity.ToTable("done_initial_db_backups", "dbo");

            entity.Property(e => e.FamilyGuid)
                .ValueGeneratedNever()
                .HasColumnName("family_guid");
        });

        modelBuilder.Entity<DropdbActivity>(entity =>
        {
            entity.HasKey(e => e.FamilyGuid).HasName("PK__dropdb_a__C30AF25AD4F3B499");

            entity.ToTable("dropdb_activity", "dbo");

            entity.Property(e => e.FamilyGuid)
                .ValueGeneratedNever()
                .HasColumnName("family_guid");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
            entity.Property(e => e.DroppedTime)
                .HasColumnType("datetime")
                .HasColumnName("dropped_time");
            entity.Property(e => e.Lifecycle)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("('DROPPED')")
                .HasColumnName("lifecycle");
        });

        modelBuilder.Entity<LogBackupManifest>(entity =>
        {
            entity.HasKey(e => new { e.FamilyGuid, e.FileEpoch }).HasName("PK__log_back__05E4F2FD20A39CEC");

            entity.ToTable("log_backup_manifest", "dbo");

            entity.HasIndex(e => e.BackupRoundId, "backup_round_index");

            entity.HasIndex(e => new { e.DatabaseName, e.RdsSequenceId }, "dbname_seqid_index");

            entity.HasIndex(e => new { e.FamilyGuid, e.RdsSequenceId }, "family_guid_seq_id_index").IsUnique();

            entity.HasIndex(e => e.FileEpoch, "file_epoch_index");

            entity.HasIndex(e => new { e.Lifecycle, e.BackupRoundId }, "lifecycle_backup_round_index");

            entity.Property(e => e.FamilyGuid).HasColumnName("family_guid");
            entity.Property(e => e.FileEpoch).HasColumnName("file_epoch");
            entity.Property(e => e.BackupFileTime)
                .HasPrecision(0)
                .HasColumnName("backup_file_time");
            entity.Property(e => e.BackupRoundId).HasColumnName("backup_round_id");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
            entity.Property(e => e.DatabaseRecoveryModel)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('FULL')")
                .HasColumnName("database_recovery_model");
            entity.Property(e => e.DatabaseState)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('ONLINE')")
                .HasColumnName("database_state");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.FirstLsn)
                .HasColumnType("numeric(25, 0)")
                .HasColumnName("first_lsn");
            entity.Property(e => e.IsLogChainBroken).HasColumnName("is_log_chain_broken");
            entity.Property(e => e.LastLsn)
                .HasColumnType("numeric(25, 0)")
                .HasColumnName("last_lsn");
            entity.Property(e => e.Lifecycle)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("lifecycle");
            entity.Property(e => e.RdsSequenceId).HasColumnName("rds_sequence_id");
            entity.Property(e => e.S3MetadataFileContent)
                .IsUnicode(false)
                .HasColumnName("s3_metadata_file_content");
        });

        modelBuilder.Entity<LoginModification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__login_mo__3213E83FB29A8A1A");

            entity.ToTable("login_modifications", "dbo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChangeStatus)
                .HasMaxLength(15)
                .HasColumnName("change_status");
            entity.Property(e => e.EventData)
                .HasColumnType("xml")
                .HasColumnName("event_data");
            entity.Property(e => e.EventType)
                .HasMaxLength(128)
                .HasColumnName("event_type");
            entity.Property(e => e.ExecutedAt)
                .HasColumnType("datetime")
                .HasColumnName("executed_at");
            entity.Property(e => e.ExtraInfo)
                .HasColumnType("xml")
                .HasColumnName("extra_info");
        });

        modelBuilder.Entity<LoginsPendingInitialPasswordChange>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK__logins_p__DDDFDD366CCECA38");

            entity.ToTable("logins_pending_initial_password_change", "dbo");

            entity.HasIndex(e => e.Name, "name_index");

            entity.Property(e => e.Sid)
                .HasMaxLength(85)
                .HasColumnName("sid");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ModifydbnameActivity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("modifydbname_activity", "dbo");

            entity.Property(e => e.DbRenamedAt)
                .HasColumnType("datetime")
                .HasColumnName("db_renamed_at");
            entity.Property(e => e.FamilyGuid).HasColumnName("family_guid");
            entity.Property(e => e.Lifecycle)
                .HasMaxLength(17)
                .HasColumnName("lifecycle");
            entity.Property(e => e.NewDbName)
                .HasMaxLength(128)
                .HasColumnName("new_db_name");
            entity.Property(e => e.OldDbName)
                .HasMaxLength(128)
                .HasColumnName("old_db_name");
        });

        modelBuilder.Entity<RdsConfiguration>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__rds_conf__72E12F1A94B7D1F5");

            entity.ToTable("rds_configuration", "dbo");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.CustomerModifiable).HasColumnName("customer_modifiable");
            entity.Property(e => e.CustomerVisible).HasColumnName("customer_visible");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.MaxValue).HasColumnName("max_value");
            entity.Property(e => e.MinValue).HasColumnName("min_value");
            entity.Property(e => e.RecordVersion).HasColumnName("record_version");
            entity.Property(e => e.Value)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<RdsCustomerTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__rds_cust__0492148DF53BD42E");

            entity.ToTable("rds_customer_tasks", "dbo");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(128)
                .HasColumnName("created_by");
            entity.Property(e => e.Cvc)
                .HasDefaultValueSql("((0))")
                .HasColumnName("cvc");
            entity.Property(e => e.DatabaseId).HasColumnName("database_id");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(128)
                .HasColumnName("database_name");
            entity.Property(e => e.FamilyGuid).HasColumnName("family_guid");
            entity.Property(e => e.Filepath).HasColumnName("filepath");
            entity.Property(e => e.KmsMasterKeyArn)
                .HasMaxLength(2048)
                .HasColumnName("KMS_master_key_arn");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.Lifecycle)
                .HasMaxLength(50)
                .HasColumnName("lifecycle");
            entity.Property(e => e.NewDatabaseName)
                .HasMaxLength(128)
                .HasColumnName("new_database_name");
            entity.Property(e => e.OverwriteFile).HasColumnName("overwrite_file");
            entity.Property(e => e.OverwriteS3BackupFile)
                .HasDefaultValueSql("((0))")
                .HasColumnName("overwrite_S3_backup_file");
            entity.Property(e => e.S3ObjectArn)
                .HasMaxLength(2048)
                .HasColumnName("S3_object_arn");
            entity.Property(e => e.ServerName)
                .HasMaxLength(128)
                .HasDefaultValueSql("(CONVERT([nvarchar],serverproperty('MachineName')))")
                .HasColumnName("server_name");
            entity.Property(e => e.TaskInfo).HasColumnName("task_info");
            entity.Property(e => e.TaskMetadata)
                .HasColumnType("xml")
                .HasColumnName("task_metadata");
            entity.Property(e => e.TaskProgress)
                .HasDefaultValueSql("((0))")
                .HasColumnName("task_progress");
            entity.Property(e => e.TaskType)
                .HasMaxLength(50)
                .HasColumnName("task_type");
        });

        modelBuilder.Entity<RdsOptionInfo>(entity =>
        {
            entity.HasKey(e => e.OptionName);

            entity.ToTable("rds_option_info", "dbo");

            entity.Property(e => e.OptionName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("option_name");
            entity.Property(e => e.ChangeState)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("change_state");
            entity.Property(e => e.InstallEndEpoch).HasColumnName("install_end_epoch");
            entity.Property(e => e.InstallStartEpoch).HasColumnName("install_start_epoch");
            entity.Property(e => e.Lifecycle)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("lifecycle");
            entity.Property(e => e.MajorEngineVersion)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("major_engine_version");
            entity.Property(e => e.Port).HasColumnName("port");
        });

        modelBuilder.Entity<RdsOptionSettingsInfo>(entity =>
        {
            entity.HasKey(e => new { e.OptionName, e.OptionSetting });

            entity.ToTable("rds_option_settings_info", "dbo");

            entity.Property(e => e.OptionName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("option_name");
            entity.Property(e => e.OptionSetting)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("option_setting");
            entity.Property(e => e.OptionSettingValue)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("option_setting_value");

            entity.HasOne(d => d.OptionNameNavigation).WithMany(p => p.RdsOptionSettingsInfos)
                .HasForeignKey(d => d.OptionName)
                .HasConstraintName("FK_rds_option_settings_option_name");
        });

        modelBuilder.Entity<RdsReadreplicaLag>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("rds_readreplica_lag", "dbo");

            entity.Property(e => e.AgName)
                .HasMaxLength(128)
                .HasColumnName("ag_name");
            entity.Property(e => e.LagSeconds).HasColumnName("lag_seconds");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<RdsSystemDatabaseObjectSyncInfo>(entity =>
        {
            entity.HasKey(e => e.ObjectClass).HasName("PK__rds_syst__39F6227834017F33");

            entity.ToTable("rds_system_database_object_sync_info", "dbo");

            entity.Property(e => e.ObjectClass)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("object_class");
            entity.Property(e => e.LastInSyncTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("last_in_sync_time");
            entity.Property(e => e.ObjectHash).HasColumnName("object_hash");
            entity.Property(e => e.SyncLifecycle)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('NONE')")
                .HasColumnName("sync_lifecycle");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
