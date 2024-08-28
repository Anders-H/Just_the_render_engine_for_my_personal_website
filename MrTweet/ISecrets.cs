namespace MrTweet;

public interface ISecrets
{
    string Target { get; }
    string Username { get; }
    string Password { get; }
    string OutputPath { get; }
    string OutputLog { get; }
}