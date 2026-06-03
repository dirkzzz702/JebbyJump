namespace JebbyJump.Analytics
{
    // Central catalog of analytics event names. Keeping them here (instead
    // of scattered string literals) makes the event surface stable and
    // provider-ready. Values are the canonical snake_case wire names and
    // must not change once a real provider is attached.
    public static class AnalyticsEvents
    {
        public const string AppSessionStarted          = "app_session_started";

        public const string MainMenuContinueClicked    = "main_menu_continue_clicked";
        public const string MainMenuLevelSelectClicked  = "main_menu_level_select_clicked";
        public const string MainMenuSettingsOpened      = "main_menu_settings_opened";
        public const string LevelSelectOpened           = "level_select_opened";
        public const string LevelSelected               = "level_selected";

        public const string LevelStarted                = "level_started";
        public const string MemoryPhaseStarted          = "memory_phase_started";
        public const string GameplayStarted             = "gameplay_started";
        public const string LevelCompleted              = "level_completed";
        public const string BestTimeImproved            = "best_time_improved";
        public const string LevelFailed                 = "level_failed";
        public const string PlayerDamaged               = "player_damaged";

        public const string SkillUsed                   = "skill_used";

        public const string PauseOpened                 = "pause_opened";
        public const string PauseResumed                = "pause_resumed";
        public const string PauseRestartClicked         = "pause_restart_clicked";
        public const string PauseMainMenuClicked        = "pause_main_menu_clicked";
        public const string PauseSettingsOpened         = "pause_settings_opened";

        public const string SettingsChanged             = "settings_changed";
    }

    // Central catalog of analytics parameter keys. Same stability contract
    // as event names: these are wire keys, kept snake_case.
    public static class AnalyticsParams
    {
        public const string SessionId          = "session_id";
        public const string LevelIndex         = "level_index";
        public const string LevelNumber        = "level_number";
        public const string TargetLevelIndex   = "target_level_index";
        public const string TargetLevelNumber  = "target_level_number";
        public const string Source             = "source";
        public const string SequenceLength     = "sequence_length";
        public const string ElapsedTime        = "elapsed_time";
        public const string Rank               = "rank";
        public const string IsNewBest          = "is_new_best";
        public const string OldBestTime        = "old_best_time";
        public const string NewBestTime        = "new_best_time";
        public const string ImprovementSeconds = "improvement_seconds";
        public const string Reason             = "reason";
        public const string RemainingLives     = "remaining_lives";
        public const string SkillType          = "skill_type";
        public const string SettingName        = "setting_name";
        public const string Value              = "value";
        public const string IsReplay           = "is_replay";
        public const string HasBestTime        = "has_best_time";
    }
}
