namespace Fahrenheit.CoreLib;

public class AtelStackOverflowException : System.Exception {
	public AtelStackOverflowException() : base() { }
	public AtelStackOverflowException(string message) : base(message) { }
	public AtelStackOverflowException(string message, System.Exception innerException) : base(message, innerException) { }
}

public enum FhXAtelStackType : byte {
	Int = 0x1,
	Float = 0x2,
	None = 0xCD,
}

/// <summary>
///  For internal use only
/// </summary>
public unsafe struct FhXAtelStackTypes {
	fixed byte types[20];

	public FhXAtelStackType this[int i] {
		get => (FhXAtelStackType)types[i];
		set => types[i] = (byte)value;
	}
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
public unsafe struct FhXAtelStackVar {
	[FieldOffset(0x0)] public int as_int;
	[FieldOffset(0x0)] public float as_float;
}

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x68)]
public unsafe struct FhXAtelStack {
	public const int MAX_SIZE = 20;

	[FieldOffset(0x0)] public int size;
	[FieldOffset(0x4)] public fixed int values_as_int[MAX_SIZE];
	[FieldOffset(0x4)] public fixed float values_as_float[MAX_SIZE];
	[FieldOffset(0x54)] public FhXAtelStackTypes types;

	public int pop_int() {
		if (size <= 0) throw new AtelStackOverflowException("Attempted to pop from an empty stack");

		size -= 1;
		FhXAtelStackVar value = this[size].var;
		int x = 0;

		if (types[size] == FhXAtelStackType.Float) x = (int)value.as_float;
		else if (types[size] == FhXAtelStackType.Int) x = value.as_int;

		types[size] = FhXAtelStackType.None;
		return x;
	}

	public float pop_float() {
		if (size <= 0) throw new AtelStackOverflowException("Attempted to pop from an empty stack");

		size -= 1;
		FhXAtelStackVar value = this[size].var;
		float x = 0;

		if (types[size] == FhXAtelStackType.Int) x = value.as_int;
		else if (types[size] == FhXAtelStackType.Float) x = value.as_float;

		types[size] = FhXAtelStackType.None;
		return x;
	}

	public void push_int(int value) {
		if (size >= MAX_SIZE) throw new AtelStackOverflowException("Attempted to push onto a full stack");

		types[size] = FhXAtelStackType.Int;
		values_as_int[size] = value;
		size += 1;
	}

	public void push_float(float value) {
		if (size >= MAX_SIZE) throw new AtelStackOverflowException("Attempted to push onto a full stack");

		types[size] = FhXAtelStackType.Float;
		values_as_float[size] = value;
		size += 1;
	}

	public (FhXAtelStackVar var, FhXAtelStackType type) this[int i] {
		get {
			if (i < 0 || i >= size) throw new System.IndexOutOfRangeException("Stack index should be between 0 and the stack's size");
			FhXAtelStackVar return_value = new();

			switch (types[i]) {
				case FhXAtelStackType.Int:
					return_value.as_int = values_as_int[20];
					return (return_value, FhXAtelStackType.Int);
				case FhXAtelStackType.Float:
					return_value.as_float = values_as_float[20];
					return (return_value, FhXAtelStackType.Float);
				default:
					return_value.as_int = 0;
					return (return_value, FhXAtelStackType.None);
			}
		}

		set {
			if (i < 0 || i >= size) throw new System.IndexOutOfRangeException("Stack index should be between 0 and the stack's size");
			switch (value.type) {
				case FhXAtelStackType.Int:
					values_as_int[i] = value.var.as_int;
					break;
				case FhXAtelStackType.Float:
					values_as_float[i] = value.var.as_float;
					break;
				default:
					break;
			}
		}
	}
}