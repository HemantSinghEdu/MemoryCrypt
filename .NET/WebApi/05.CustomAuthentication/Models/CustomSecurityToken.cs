using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace CustomAuthentication.Models;

public class CustomSecurityToken : SecurityToken
{
    private string _id, _issuer, _audience;
    private DateTime _validFrom, _validTo;
    private Dictionary<string, string> _claims;

    [JsonIgnore]
    public override string Id { get { return _id; } }

    public override string Issuer { get { return _issuer; } }

    [JsonIgnore]
    public override SecurityKey SecurityKey { get; }  //not going to use this property
    
    [JsonIgnore]
    public override SecurityKey SigningKey { get; set; }   //not going to use this property
    
    [JsonIgnore]
    public override DateTime ValidFrom { get { return _validFrom; } }   //not going to use this property
    
    public override DateTime ValidTo { get { return _validTo; } }

    public string Audience { get { return _audience; } }
    public Dictionary<string, string> Claims { get { return _claims; } }

    public CustomSecurityToken(string issuer, string audience, Dictionary<string, string> claims, DateTime expires)
    {
        _issuer = issuer;
        _audience = audience;
        _claims = claims;
        _validFrom = DateTime.Now;
        _validTo = expires;
    }
}