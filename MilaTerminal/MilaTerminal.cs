using Rocket.API.Collections;
using Rocket.Core.Plugins;
using System;
using System.IO;
using System.Reflection;
using MilaTerminal.Services;
using Logger = Rocket.Core.Logging.Logger;

namespace MilaTerminal
{
    public class MilaTerminal : RocketPlugin<MilaTerminalConfiguration>
    {
        public static MilaTerminal Instance { get; private set; }
        public static string CurrentWorkingDirectory { get; set; }
        public static string ServerRootPath { get; private set; }

        protected override void Load()
        {
            Instance = this;
            ServerRootPath = System.IO.Directory.GetCurrentDirectory();

            CurrentWorkingDirectory = ServerRootPath;

            AuthService.Initialize();

            Logger.Log("");
            Logger.Log("=============================================", ConsoleColor.Yellow);
            Logger.Log("       ███╗   ███╗", ConsoleColor.Magenta);
            Logger.Log("       ████╗ ████║", ConsoleColor.Magenta);
            Logger.Log("       ██╔████╔██║", ConsoleColor.Magenta);
            Logger.Log("       ██║╚██╔╝██║", ConsoleColor.Magenta);
            Logger.Log("       ██║ ╚═╝ ██║", ConsoleColor.Magenta);
            Logger.Log("       ╚═╝     ╚═╝", ConsoleColor.Magenta);
            Logger.Log("---------------------------------------------", ConsoleColor.Yellow);
            Logger.Log($"       Plugin: {Name}", ConsoleColor.White);
            Logger.Log("       Created by: Mila", ConsoleColor.White);
            Logger.Log($"       Version: {Assembly.GetName().Version}", ConsoleColor.White);
            Logger.Log("=============================================", ConsoleColor.Yellow);

            Logger.Log(Instance.Translate("load_success"));
            Logger.Log(Instance.Translate("load_help_prompt"));
        }

        protected override void Unload()
        {
            AuthService.Shutdown();

            Logger.Log(Instance.Translate("unload_success"));

            Instance = null;
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "load_success", ">> MilaTerminal loaded successfully." },
            { "load_help_prompt", ">> Type '/mroot help' to see the list of available commands." },
            { "unload_success", ">> MilaTerminal has been unloaded." },
            { "auth_service_initialized", "Authentication Service initialized with provider: {0}" },
            { "syntax_error", ">> Syntax: /mroot {0} {1}" },
            { "command_not_found", ">> Subcommand '{0}' not found." },
            { "error_generic", ">> An unexpected error occurred." },
            
            { "session_expired", ">> Session expired due to inactivity." },
            { "login_required_prompt_1", ">> You are not logged in or your session has expired." },
            { "login_required_prompt_2", ">> Use '/mroot login <user> <password>' to access." },
            { "access_denied", ">> Access denied. You do not have permission to use this command." },
            { "login_success", "✅ Welcome, {0}! Session started successfully." },
            { "login_invalid_credentials", ">> Error: Incorrect username or password." },
            { "logout_success", "✅ Session closed successfully." },
            { "password_update_success", "✅ Password updated successfully." },
            { "password_update_fail", ">> An error occurred while trying to change the password." },
            
            { "user_list_header", "--- 👥 List of Registered Users ---" },
            { "user_list_footer", "------------------------------------" },
            { "user_list_format", "- User: {0} | Permissions: {1}" },
            { "user_list_format_simple", "- {0}" },
            { "user_list_perms_all", "ALL" },
            { "user_list_perms_none", "NONE" },
            { "error_get_current_user", "Error: Could not verify the current session." },
            { "useradd_denied", ">> Access denied. Only 'root' can create users." },
            { "useradd_success", "✅ User '{0}' created successfully." },
            { "useradd_exists", ">> Error: User '{0}' already exists." },
            { "userdel_denied", ">> Access denied. Only 'root' can delete users." },
            { "userdel_success", "✅ User '{0}' deleted successfully." },
            { "userdel_fail", ">> Error: User '{0}' does not exist or could not be deleted." },
            { "userdel_no_self", ">> Error: You cannot delete your own account while logged in." },
            { "userdel_no_root", ">> Error: The 'root' user cannot be deleted." },
            { "usermod_denied", ">> Access denied. Only 'root' can modify users." },
            { "usermod_success", "✅ Permissions for user '{0}' updated." },
            { "usermod_fail", ">> Error: Could not modify user '{0}'. Check the name and mode (add/remove/set)." },
            { "usermod_no_root", ">> Error: Permissions for the 'root' user cannot be modified." },

            { "file_protected", ">> Protected! Deleting MilaTerminal configuration files is not allowed." },
            { "folder_protected", ">> Protected! You cannot delete the main MilaTerminal folder." },
            { "folder_critical", ">> DANGER! The folder '{0}' is critical and its deletion has been blocked." },
            { "security_error_path", ">> Security Error: Access to directories outside the server folder is not allowed." },
            { "error_file_is_folder", ">> Error: '{0}' is a folder. Use the appropriate command." },
            { "error_not_found", ">> Error: Path '{0}' was not found." },
            { "delete_file_success", "✅ [Success] File '{0}' has been deleted." },
            { "delete_file_fail", ">> FAILED: An error occurred while deleting the file. Is it in use?" },
            { "delete_folder_warning", "WARNING: You are about to permanently delete the folder '{0}' and all its contents." },
            { "delete_folder_success", "✅ [Success] The folder '{0}' has been deleted." },
            { "delete_folder_fail", ">> FAILED: Could not delete the folder. Probable cause: The folder or one of its files is in use." },
            { "zip_compressing", "🗜️ Compressing '{0}'..." },
            { "zip_success", "✅ [Success] Compression finished!" },
            { "zip_fail", ">> FAILED: An error occurred during compression." },
            { "cat_header", "--- 📄 Contents of {0} ---" },
            { "cat_footer", "--- End of file ---" },
            { "cat_too_long", ">> ... File too long. Showing only the first {0} lines. ..." },
            { "cat_fail", ">> Could not read the file. It may not be a text file or it might be in use." },
            { "ls_header", "--- 📂 Contents of: {0} ---" },
            { "ls_folder", "📁 [Folder] {0}" },
            { "ls_file", "📄 [File]   {0}" },
            { "ls_empty", "The directory is empty." },
            { "ls_footer", "-------------------------------------------" },
            { "find_searching", "🔎 Searching for '{0}' in '{1}' and subfolders..." },
            { "find_none_found", "No files matching the search criteria were found." },
            { "find_found_header", "Found {0} files:" },
            { "find_found_item", "- {0}" },
            { "mkdir_success", "✅ [Success] Folder created: {0}" },
            { "mkdir_fail", ">> An error occurred while trying to create the folder." },
            { "mkdir_exists", ">> Error: A file or folder with the name '{0}' already exists." },
            { "mv_success_file", "✅ [Success] File moved/renamed to: {0}" },
            { "mv_success_folder", "✅ [Success] Folder moved/renamed to: {0}" },
            { "mv_fail", ">> An error occurred during the operation. Check paths and permissions." },
            { "cd_success", ">> Current directory: {0}" },
            
            { "help_header", "--- 🦾 MilaTerminal's Command Swiss Army Knife ---" },
            { "help_usage", "Usage: /mroot <command> [arguments]" },
            { "help_nav_header", "NAVIGATION & VIEWING:" },
            { "help_nav_ls", "  ls [path]      - Lists the content of the current directory or a path." },
            { "help_nav_cd", "  cd <path>      - Changes to the specified directory (use '..' to go back)." },
            { "help_nav_cat", "  cat <file>     - Displays the content of a text file." },
            { "help_create_header", "CREATION & DELETION:" },
            { "help_create_mkdir", "  mkdir <name>   - Creates a new folder in the current directory." },
            { "help_create_delf", "  delf <folder>  - CAUTION! Deletes a folder and all its content." },
            { "help_create_delfile", "  delfile <file> - Deletes a specific file." },
            { "help_manage_header", "MANAGEMENT & UTILITIES:" },
            { "help_manage_mv", "  mv <from> <to> - Moves or renames a file or folder." },
            { "help_manage_find", "  find <pattern> - Searches for files (e.g., find *.xml, find Player.dat)." },
            { "help_manage_zip", "  zip <from> <to>- Compresses a folder into a .zip file." },
            { "help_footer", "-------------------------------------------------" }
        };
    }
}