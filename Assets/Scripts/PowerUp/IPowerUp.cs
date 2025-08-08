namespace PowerUp
{
    public interface IPowerUp
    {
        public PowerUpEnum PowerUpType { get; }
        public void UsePowerUp(int level);
    }
}