using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DSChat")]
[assembly: AssemblyDescription("DSChat")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Ken Anderson")]
[assembly: AssemblyProduct("DSChat")]
[assembly: AssemblyCopyright("Copyright ©  2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6304724b-1b45-4634-b65a-24b52715d754")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.3.1.0")]
[assembly: AssemblyFileVersion("1.3.1.0")]

// Major: 1
// Minor: 0
// Build: 0
// Revision: 0
// NOTES: Original program.
// Allowed simple communication but the chatform was buggy if someone sent a message before a chatform was created.

// Major: 1
// Minor: 1
// Build: 0
// Revision: 0
// NOTES: Bug fixes with new features.
// Fixed the issue with the chatform by creating a ChatIntermediary and caching the messages with the client until the form was created.
// Add 0.20% opacity when the form was out of focus.
// Implemented the passphrase security option & security models in the NodeSystem.
// Implemented all exception classes.

// Major: 1
// Minor: 1
// Build: 0
// Revision: 1
// NOTES: Bug fixes with new features.
// Fixed a bug with characters that are used in HTML tags by converting them to the appropriate symbolic representation.
// Updated the following events with icons.
// Disconnected from the parent server or server not running -- Red bunny.
// Incoming message on chatform -- Green bunny.
// Server connected -- White bunny.
// Changed the color scheme in the chatform so that the current user and the chatter have two distinct colors.

// Major: 1
// Minor: 2
// Build: 0
// Revision: 1
// NOTES: Modified with new features.
// Updated the buzz event to create a red, strong HTML tag around the incoming message.
// Updated the main DSChat form to indicate an incoming message with a green bunny.
// Removed the text DSChat from the chatform's title.

// Major: 1
// Minor: 3
// Build: 0
// Revision: 0
// NOTES: Modified with new features.
// Added the ability to change the opacity of the chatform when in focus.
// Added a debug option to the DSChat form when compiled in debug mode.
// Added an action menu that allows for connecting/disconnecting, and the option to clear the status window.
// The opacity is a global setting for now.

// Major: 1
// Minor: 3
// Build: 1
// Revision: 0
// NOTES: Bug fixes
// Fixed a bug introduced when cleaning a list of chats when disconnected.
// Fixed an issue with the opacity not being applied properly.
// Added a connection timeout thread.

