namespace ImageResizeUpload.Helper
{
    public static class ImageSize
    {
        public static string Get(string url, string resize)
        {
            //"~/images/2022/03/jlkfadlkfjkalf-7890-thumb.jpg" 150*150
            //"~/images/2022/03/jlkfadlkfjkalf-7890-medium.jpg" 400*550
            //"~/images/2022/03/jlkfadlkfjkalf-7890-fullscreen.jpg" 1200*
            //"~/images/2022/03/jlkfadlkfjkalf-7890.jpg"

            if (url != null)
            {
                string imgPath = url.Split('.')[0];
                string ext = url.Split('.')[1];


                switch (resize)
                {
                    case "thumb":
                        return "https://localhost:7091" + imgPath + "-thumb." + ext;
                    case "medium":
                        return "https://localhost:7091" + imgPath + "-medium." + ext;
                    case "fullscreen":
                        return "https://localhost:7091" + imgPath + "-fullscreen." + ext;
                    default:
                      return "https://localhost:7091" + imgPath + ext;

                }
            }
            else
            {
                return "";
            }

        }
    }
}
