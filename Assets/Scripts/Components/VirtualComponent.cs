using CustomTools;

public abstract class VirtualComponent {
	public abstract IVector3 MoveElectron ();
	public abstract void Compute(bool input, out bool output);
}
