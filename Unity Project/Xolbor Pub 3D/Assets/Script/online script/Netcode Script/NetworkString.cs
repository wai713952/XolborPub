using Unity.Collections;
using Unity.Netcode;

public struct NetworkString : INetworkSerializable
{
    //network string is used to convert normal string into serializeValue that can be send as online variable
    //don't need to understand the code, just using it

    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) =>
        new NetworkString() { info = new FixedString32Bytes(s) };
}