using System;
using System.Collections.Generic;
using System.Diagnostics;

using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class NewGameTransition : Transition
{
    public List<(string, byte)> startGameText = null;

    private byte textColour = 0x08;

    private Dictionary<char, (byte?, byte)> characterEncodingLatin = new Dictionary<char, (byte?, byte)>()
    {
        {'0', (null ,0x30)},
        {'1', (null ,0x31)},
        {'2', (null ,0x32)},
        {'3', (null ,0x33)},
        {'4', (null ,0x34)},
        {'5', (null ,0x35)},
        {'6', (null ,0x36)},
        {'7', (null ,0x37)},
        {'8', (null ,0x38)},
        {'9', (null ,0x39)},
        {' ', (null, 0x3A)},
        {'.', (null, 0x48)},
        {'?', (null, 0x4F)},
        {'A', (null, 0x50)},
        {'B', (null, 0x51)},
        {'C', (null, 0x52)},
        {'D', (null, 0x53)},
        {'E', (null, 0x54)},
        {'F', (null, 0x55)},
        {'G', (null, 0x56)},
        {'H', (null, 0x57)},
        {'I', (null, 0x58)},
        {'J', (null, 0x59)},
        {'K', (null, 0x5A)},
        {'L', (null, 0x5B)},
        {'M', (null, 0x5C)},
        {'N', (null, 0x5D)},
        {'O', (null, 0x5E)},
        {'P', (null, 0x5F)},
        {'Q', (null, 0x60)},
        {'R', (null, 0x61)},
        {'S', (null, 0x62)},
        {'T', (null, 0x63)},
        {'U', (null, 0x64)},
        {'V', (null, 0x65)},
        {'W', (null, 0x66)},
        {'X', (null, 0x67)},
        {'Y', (null, 0x68)},
        {'Z', (null, 0x69)},
        {'[', (null, 0x6A)},
        //{'???', (null, 0x6B)},
        {']', (null, 0x6C)},
        {'a', (null, 0x70)},
        {'b', (null, 0x71)},
        {'c', (null, 0x72)},
        {'d', (null, 0x73)},
        {'e', (null, 0x74)},
        {'f', (null, 0x75)},
        {'g', (null, 0x76)},
        {'h', (null, 0x77)},
        {'i', (null, 0x78)},
        {'j', (null, 0x79)},
        {'k', (null, 0x7A)},
        {'l', (null, 0x7B)},
        {'m', (null, 0x7C)},
        {'n', (null, 0x7D)},
        {'o', (null, 0x7E)},
        {'p', (null, 0x7F)},
        {'q', (null, 0x80)},
        {'r', (null, 0x81)},
        {'s', (null, 0x82)},
        {'t', (null, 0x83)},
        {'u', (null, 0x84)},
        {'v', (null, 0x85)},
        {'w', (null, 0x86)},
        {'x', (null, 0x87)},
        {'y', (null, 0x88)},
        {'z', (null, 0x89)},
    };

    private Dictionary<char, (byte?, byte)> characterEncodingJapanese = new Dictionary<char, (byte?, byte)>()
    {
        {'0', (null ,0x30)},
        {'1', (null ,0x31)},
        {'2', (null ,0x32)},
        {'3', (null ,0x33)},
        {'4', (null ,0x34)},
        {'5', (null ,0x35)},
        {'6', (null ,0x36)},
        {'7', (null ,0x37)},
        {'8', (null ,0x38)},
        {'9', (null ,0x39)},
        {' ', (null ,0x3A)},
        {'!', (null, 0x3B)},
        //{'-', (null, 0x3C)}, // Long Dash
        {'~', (null, 0x3D)},
        //{'...', (null, 0x3E)}, // Ellipsis
        {'%', (null, 0x3F)},
        {'&', (null, 0x40)},
        //{'X', (null, 0x41)}, // Multiply Sign
        {'(', (null, 0x42)},
        {')', (null, 0x43)},
        {'*', (null, 0x44)},
        {'+', (null, 0x45)},
        {',', (null, 0x46)},
        {'-', (null, 0x47)},
        {'.', (null, 0x48)},
        {'/', (null, 0x49)},
        {':', (null, 0x4A)},
        {';', (null, 0x4B)},
        {'[', (null, 0x4C)},
        {'=', (null, 0x4D)},
        {']', (null, 0x4E)},
        {'?', (null, 0x4F)},
        //{'X', (null, 0x50)}, // Cross Sign
        {'A', (0x2C, 0x30)},
        {'B', (0x2C, 0x31)},
        {'C', (0x2C, 0x32)},
        {'D', (0x2C, 0x33)},
        {'E', (0x2C, 0x34)},
        {'F', (0x2C, 0x35)},
        {'G', (0x2C, 0x36)},
        {'H', (0x2C, 0x37)},
        {'I', (0x2C, 0x38)},
        {'J', (0x2C, 0x39)},
        {'K', (0x2C, 0x3A)},
        {'L', (0x2C, 0x3B)},
        {'M', (0x2C, 0x3C)},
        {'N', (0x2C, 0x3D)},
        {'O', (0x2C, 0x3E)},
        {'P', (0x2C, 0x3F)},
        {'Q', (0x2C, 0x40)},
        {'R', (0x2C, 0x41)},
        {'S', (0x2C, 0x42)},
        {'T', (0x2C, 0x43)},
        {'U', (0x2C, 0x44)},
        {'V', (0x2C, 0x45)},
        {'W', (0x2C, 0x46)},
        {'X', (0x2C, 0x47)},
        {'Y', (0x2C, 0x48)},
        {'Z', (0x2C, 0x49)},
        {'a', (0x2C, 0x4A)},
        {'b', (0x2C, 0x4B)},
        {'c', (0x2C, 0x4C)},
        {'d', (0x2C, 0x4D)},
        {'e', (0x2C, 0x4E)},
        {'f', (0x2C, 0x4F)},
        {'g', (0x2C, 0x50)},
        {'h', (0x2C, 0x51)},
        {'i', (0x2C, 0x52)},
        {'j', (0x2C, 0x53)},
        {'k', (0x2C, 0x54)},
        {'l', (0x2C, 0x55)},
        {'m', (0x2C, 0x56)},
        {'n', (0x2C, 0x57)},
        {'o', (0x2C, 0x58)},
        {'p', (0x2C, 0x59)},
        {'q', (0x2C, 0x5A)},
        {'r', (0x2C, 0x5B)},
        {'s', (0x2C, 0x5C)},
        {'t', (0x2C, 0x5D)},
        {'u', (0x2C, 0x5E)},
        {'v', (0x2C, 0x5F)},
        {'w', (0x2C, 0x60)},
        {'x', (0x2C, 0x61)},
        {'y', (0x2C, 0x62)},
        {'z', (0x2C, 0x63)},
    };

    private Dictionary<char, (byte?, byte)> characterEncodingChinese = new Dictionary<char, (byte?, byte)>()
    {
        {'0', (null ,0x30)},
        {'1', (null ,0x31)},
        {'2', (null ,0x32)},
        {'3', (null ,0x33)},
        {'4', (null ,0x34)},
        {'5', (null ,0x35)},
        {'6', (null ,0x36)},
        {'7', (null ,0x37)},
        {'8', (null ,0x38)},
        {'9', (null ,0x39)},
        {' ', (null ,0x3A)},
        {'!', (null, 0x3B)},
        //{'-', (null, 0x3C)}, // Long Dash
        {'~', (null, 0x3D)},
        //{'...', (null, 0x3E)}, // Ellipsis
        {'%', (null, 0x3F)},
        {'&', (null, 0x40)},
        //{'X', (null, 0x41)}, // Multiply Sign
        {'(', (null, 0x42)},
        {')', (null, 0x43)},
        {'*', (null, 0x44)},
        {'+', (null, 0x45)},
        {',', (null, 0x46)},
        {'-', (null, 0x47)},
        {'.', (null, 0x48)},
        {'/', (null, 0x49)},
        {':', (null, 0x4A)},
        {';', (null, 0x4B)},
        {'[', (null, 0x4C)},
        {'=', (null, 0x4D)},
        {']', (null, 0x4E)},
        {'?', (null, 0x4F)},
        //{'X', (null, 0x50)}, // Cross Sign
        {'A', (null, 0x5A)},
        {'B', (null, 0x5B)},
        {'C', (null, 0x5C)},
        {'D', (null, 0x5D)},
        {'E', (null, 0x5E)},
        {'F', (null, 0x5F)},
        {'G', (null, 0x60)},
        {'H', (null, 0x61)},
        {'I', (null, 0x62)},
        {'J', (null, 0x63)},
        {'K', (null, 0x64)},
        {'L', (null, 0x65)},
        {'M', (null, 0x66)},
        {'N', (null, 0x67)},
        {'O', (null, 0x68)},
        {'P', (null, 0x69)},
        {'Q', (null, 0x6A)},
        {'R', (null, 0x6B)},
        {'S', (null, 0x6C)},
        {'T', (null, 0x6D)},
        {'U', (null, 0x6E)},
        {'V', (null, 0x6F)},
        {'W', (null, 0x70)},
        {'X', (null, 0x71)},
        {'Y', (null, 0x72)},
        {'Z', (null, 0x73)},
        {'a', (null, 0x74)},
        {'b', (null, 0x75)},
        {'c', (null, 0x76)},
        {'d', (null, 0x77)},
        {'e', (null, 0x78)},
        {'f', (null, 0x79)},
        {'g', (null, 0x7A)},
        {'h', (null, 0x7B)},
        {'i', (null, 0x7C)},
        {'j', (null, 0x7D)},
        {'k', (null, 0x7E)},
        {'l', (null, 0x7F)},
        {'m', (null, 0x80)},
        {'n', (null, 0x81)},
        {'o', (null, 0x82)},
        {'p', (null, 0x83)},
        {'q', (null, 0x84)},
        {'r', (null, 0x85)},
        {'s', (null, 0x86)},
        {'t', (null, 0x87)},
        {'u', (null, 0x88)},
        {'v', (null, 0x89)},
        {'w', (null, 0x8A)},
        {'x', (null, 0x8B)},
        {'y', (null, 0x8C)},
        {'z', (null, 0x8D)},
    };

    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        int newLineCount;
        
        byte[] dialogueBoxBytes;
        List<byte> arrangedYesNoBytes = new List<byte> { };
        List<byte> originalYesNoBytes = new List<byte> { };
        List<byte> originalRemainingBytes = new List<byte> { };

        int newArrangedBoxLength;
        int newOriginalBoxLength;

        List<byte> modDialogueBytes = new List<byte>{ };
        Dictionary<char, (byte?, byte)> characterEncoding;

        int bytesToRead = 2048;
        dialogueBoxBytes = MemoryWatchers.DialogueFile.DeepPtr.DerefBytes(process, bytesToRead);

        byte language = MemoryWatchers.Language.Current;

        switch(language)
        {
            case 0x00: // Japanese
                characterEncoding = characterEncodingJapanese;
                break;
            case 0x09: // Korean - Same Encoding as Chinese
                characterEncoding = characterEncodingChinese;
                break;
            case 0x0A: // Chinese
                characterEncoding = characterEncodingChinese;
                break;
            default:
                characterEncoding = characterEncodingLatin;
                break;
        }

        int arrangedMessageBoxStart = dialogueBoxBytes[4 * 14] + dialogueBoxBytes[4 * 14 + 1] * 0x100;
        int originalMessageBoxStart = dialogueBoxBytes[4 * 16] + dialogueBoxBytes[4 * 16 + 1] * 0x100;
        int originalMessageBoxEnd = dialogueBoxBytes[4 * 18] + dialogueBoxBytes[4 * 18 + 1] * 0x100 - 1;

        // get the position of the final byte of starting bytes
        for (int i = originalMessageBoxEnd + 1; i < bytesToRead; i++)
        {
            if (dialogueBoxBytes[i] == 0x00)
            {
                int bytesEnd = i;
            }
        }

        // get bytes for yes / no options at the end of the arranged message box bytes
        newLineCount = 0;

        for (int i = originalMessageBoxStart - 1; i >= 0; i--)
        {
            if (dialogueBoxBytes[i] == 0x03)
            {
                newLineCount++;
            }

            if (newLineCount == 2)
            {
                break;
            }
            else
            {
                arrangedYesNoBytes.Insert(0,dialogueBoxBytes[i]);
            }
        }

        // get bytes for yes / no options at the end of the original message box bytes
        newLineCount = 0;

        for (int i = originalMessageBoxEnd; i >= 0; i--)
        {
            if (dialogueBoxBytes[i] == 0x03)
            {
                newLineCount++;
            }

            if (newLineCount == 2)
            {
                break;
            }
            else
            {
                originalYesNoBytes.Insert(0, dialogueBoxBytes[i]);
            }
        }

        // get remaining bytes after the original message box bytes

        for (int i = originalMessageBoxEnd + 1; i < 0; i--)
        {
            if (dialogueBoxBytes[i] == 0x03)
            {
                break;
            }
            else
            {
                originalRemainingBytes.Add(dialogueBoxBytes[i]);
            }
        }

        // create the ordered bytes for the dialogue boxes

        // Start of Message Box Bytes - Arranged
        modDialogueBytes.Add(0x09);
        modDialogueBytes.Add(0x30);
        modDialogueBytes.Add(0x0A);
        modDialogueBytes.Add(textColour);

        // Add lines for arranged message box
        for (int i = 0; i < startGameText.Count; i++)
        {
            byte indent = startGameText[i].Item2;
            modDialogueBytes.Add(0x07);
            modDialogueBytes.Add(indent);

            foreach (char character in startGameText[i].Item1)
            {
                (byte?, byte) encodedCharacter = characterEncoding[character];
                if (!(encodedCharacter.Item1 is null))
                {
                    modDialogueBytes.Add((byte)encodedCharacter.Item1);
                }
                modDialogueBytes.Add(encodedCharacter.Item2);
            }
            modDialogueBytes.Add(0x03);
        }

        for (int i = 0; i < arrangedYesNoBytes.Count; i++)
        {
            modDialogueBytes.Add(arrangedYesNoBytes[i]);
        }

        newArrangedBoxLength = modDialogueBytes.Count;

        // Start of Message Box Bytes - Original
        modDialogueBytes.Add(0x09);
        modDialogueBytes.Add(0x30);
        modDialogueBytes.Add(0x0A);
        modDialogueBytes.Add(textColour);

        // Add lines for original message box
        for (int i = 0; i < startGameText.Count; i++)
        {
            byte indent = startGameText[i].Item2;
            modDialogueBytes.Add(0x07);
            modDialogueBytes.Add(indent);

            foreach (char character in startGameText[i].Item1)
            {
                (byte?, byte) encodedCharacter = characterEncoding[character];
                if (!(encodedCharacter.Item1 is null))
                {
                    modDialogueBytes.Add((byte)encodedCharacter.Item1);
                }
                modDialogueBytes.Add(encodedCharacter.Item2);
            }
            modDialogueBytes.Add(0x03);
        }

        for (int i = 0; i < originalYesNoBytes.Count; i++)
        {
            modDialogueBytes.Add(originalYesNoBytes[i]);
        }

        newOriginalBoxLength = modDialogueBytes.Count - newArrangedBoxLength;

        // Add lines for remaining bytes after original soundtrack message box
        for (int i = 0; i < originalRemainingBytes.Count; i++)
        {
            modDialogueBytes.Add(originalRemainingBytes[i]);
        }

        // Update body data with message box new bytes

        for (int i = 0; i < modDialogueBytes.Count; i++)
        {
            try 
            {
                dialogueBoxBytes[arrangedMessageBoxStart + i] = modDialogueBytes[i];
            }
            catch(IndexOutOfRangeException e)
            {
                DiagnosticLog.Information("Exception: " + e.Message);
            }
        }

        // Update header with message box start locations

        dialogueBoxBytes[4 * 16] = (byte)((arrangedMessageBoxStart + newArrangedBoxLength) & 0xFF);
        dialogueBoxBytes[4 * 16 + 1] = (byte)(((arrangedMessageBoxStart + newArrangedBoxLength) & 0xFF00) >> 8);
        dialogueBoxBytes[4 * 17] = (byte)((arrangedMessageBoxStart + newArrangedBoxLength) & 0xFF);
        dialogueBoxBytes[4 * 17 + 1] = (byte)(((arrangedMessageBoxStart + newArrangedBoxLength) & 0xFF00) >> 8);
        dialogueBoxBytes[4 * 18] = (byte)((arrangedMessageBoxStart + newArrangedBoxLength + newOriginalBoxLength) & 0xFF);
        dialogueBoxBytes[4 * 18 + 1] = (byte)(((arrangedMessageBoxStart + newArrangedBoxLength + newOriginalBoxLength) & 0xFF00) >> 8);
        dialogueBoxBytes[4 * 19] = (byte)((arrangedMessageBoxStart + newArrangedBoxLength + newOriginalBoxLength) & 0xFF);
        dialogueBoxBytes[4 * 19 + 1] = (byte)(((arrangedMessageBoxStart + newArrangedBoxLength + newOriginalBoxLength) & 0xFF00) >> 8);

        new Transition { ForceLoad = false, Description = "New Game - Version Information", DialogueFile = dialogueBoxBytes }.Execute();

    }
}
