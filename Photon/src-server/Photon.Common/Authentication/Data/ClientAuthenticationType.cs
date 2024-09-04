
namespace Photon.Common.Authentication.Data
{
    public enum ClientAuthenticationType : byte
    {
        Custom = 0,
        Steam = 1,
        Facebook = 2,
        Oculus = 3,
        PlayStation4 = 4,
        Xbox = 5,
        PlayerIo = 8,
        Jwt = 9,
        Viveport = 10,
        Nintendo = 11,
        PlayStation5 = 12,
        Epic = 13,
        Gamepass = 14,
        FacebookGaming = 15,
        None = 254, //use 254 to be safe because 255 is used for token auth
    }
}
