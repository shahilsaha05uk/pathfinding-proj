    public partial class Controller
    {
        public void Save()
        {
            var data = CreateSaveData(mGrid.GetGridConfig());
            saveManager.Save(data);
        }
        public void Export()
        {
            saveManager.Export("saveData.json");
        }
        public void SaveAndExport()
        {
            var data = CreateSaveData(mGrid.GetGridConfig());
            saveManager.SaveAndExport(data, "saveData.json");
        }
        public void ClearData()
        {
            saveManager.Clear();
        }
    
        private SaveData CreateSaveData(GridConfig config)
        {
            return new SaveData
            {
                GridSize = config.GridSize,
                MaxHeight = config.MaxHeight,
                Offset = config.Offset,
                OffsetRandomization = config.OffsetRandomization,
                NoiseScale = config.NoiseScale,
                // ObstacleDensity = config.ObstacleDensity,
                // AStarTime = config.AStarTime,
                // GBFSTime = config.GBFSTime,
                // ILSWithAStarTime = config.ILSWithAStarTime,
                //
                // AStarSpace = config.AStarSpace,
                // GBFSSpace = config.GBFSSpace,
                // ILSWithAStarSpace = config.ILSWithAStarSpace,
                //
                // ILSIterations = config.ILSIterations
            };
        }
    }
