using CustomTools;

public abstract class VirtualComponent {
	public abstract IVector3 MoveElectron ();
	public abstract IVector3 MoveSecElectron();
	public abstract void Compute(bool input, out bool output);
	public abstract void Compute(out bool output);
}
