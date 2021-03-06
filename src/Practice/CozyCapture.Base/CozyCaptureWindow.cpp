#include "CozyCaptureWindow.h"

CozyCaptureWindow::CozyCaptureWindow(DWORD dwFlags, LPCTSTR lpFileName, LPDWORD lpResultState)
    :m_lpFileName(lpFileName), m_lpResultState(lpResultState)
{
    if (dwFlags | FLG_TOCLIP)
    {
        m_IsSaveToClipboard = true;
    }
    if (dwFlags | FLG_TOFILE)
    {
        m_IsSaveToFile = true;
    }
    *m_lpResultState = RET_FAILED;
}

CozyCaptureWindow::~CozyCaptureWindow()
{

}

LRESULT CozyCaptureWindow::OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    HWND hWnd   = ::GetDesktopWindow();
    HDC hdc     = ::GetWindowDC(hWnd);
    RECT rect;
    ::GetWindowRect(hWnd, &rect);

    m_lWidth    = rect.right - rect.left;
    m_lHeight   = rect.bottom - rect.top;
    MoveWindow(0, 0, m_lWidth, m_lHeight);

    SetWindowPos(HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

    HBITMAP hBmp = ::CreateCompatibleBitmap(hdc, m_lWidth, m_lHeight);

    BITMAP bmp;
    if (!::GetObject(hBmp, sizeof(BITMAP), (LPVOID)&bmp))
    {
        return 0;
    }

    int nBPP    = bmp.bmPlanes * bmp.bmBitsPixel;
    nBPP        = nBPP < 24 ? 24 : 32;

    if (!m_CaptureImg.Create(m_lWidth, m_lHeight, nBPP))
    {
        return 0;
    }
    if (!m_ResultImg.Create(m_lWidth, m_lHeight, nBPP))
    {
        return 0;
    }
    if (!m_MaskImg.Create(m_lWidth, m_lHeight, nBPP))
    {
        return 0;
    }

    CImageDC maskDc(m_MaskImg);

    RECT r;
    r.left      = 0;
    r.top       = 0;
    r.bottom    = m_lHeight;
    r.right     = m_lWidth;
    ::FillRect(maskDc, &r, ::CreateSolidBrush(RGB(0, 0, 0)));

    CImageDC imageDc(m_CaptureImg);
    ::BitBlt(imageDc, 0, 0, m_lWidth, m_lHeight, hdc, 0, 0, SRCCOPY);

    return 0;
}

LRESULT CozyCaptureWindow::OnMouseMove(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    if (m_ThisStatus == CaptureStatus::S_Selecting)
    {
        if (!m_IsMoved) m_IsMoved = true;
        m_CurrPoint = POINT{ GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    }
    
    return 0;
}

LRESULT CozyCaptureWindow::OnLeftButtonUp(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    m_ThisStatus = CaptureStatus::S_Selected;

    return 0;
}

LRESULT CozyCaptureWindow::OnLeftButtonDown(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    if (m_ThisStatus == CaptureStatus::S_None)
    {
        m_IsMoved       = false;
        m_ThisStatus    = CaptureStatus::S_Selecting;
        m_BeginPoint    = POINT{ GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    }
    else if (m_ThisStatus == CaptureStatus::S_Selected)
    {
        RECT ActRect;
        Point2Rect(m_BeginPoint, m_CurrPoint, &ActRect);

        CImage OutputImg;
        OutputImg.Create(ActRect.right - ActRect.left, ActRect.bottom - ActRect.top, m_CaptureImg.GetBPP());

        {
            CImageDC outputDc(OutputImg);
            m_CaptureImg.BitBlt(outputDc, 0, 0, ActRect.right - ActRect.left, ActRect.bottom - ActRect.top, ActRect.left, ActRect.top);
        }
        if (m_IsSaveToFile)
        {
            if (SendImageToFile(OutputImg))
            {
                *m_lpResultState |= RET_FILE;
            }
        }
        if (m_IsSaveToClipboard)
        {
            if (SendImageToClipboard(OutputImg))
            {
                *m_lpResultState |= RET_CLIP;
            }
        }
        SetWindowPos(HWND_BOTTOM, 0, 0, 0, 0, SWP_HIDEWINDOW);
        Exit();
    }
    return 0;
}

bool CozyCaptureWindow::SendImageToClipboard(CImage &Img)
{
    if (OpenClipboard())
    {
        HBITMAP hbitmap_dib = Img.Detach();
        if (!hbitmap_dib)
        {
            return false;
        }

        DIBSECTION ds;
        ::GetObject(hbitmap_dib, sizeof(DIBSECTION), &ds);
        ds.dsBmih.biCompression = BI_RGB;

        HDC hdc             = ::GetDC(NULL);
        HBITMAP hbitmap_ddb = ::CreateDIBitmap(hdc, &ds.dsBmih, CBM_INIT, ds.dsBm.bmBits, (BITMAPINFO*)&ds.dsBmih, DIB_RGB_COLORS);
        ::ReleaseDC(NULL, hdc);

        ::EmptyClipboard();
        ::SetClipboardData(CF_BITMAP, hbitmap_ddb);
        ::CloseClipboard();
        return true;
    }
    return false;
}

bool CozyCaptureWindow::SendImageToFile(const CImage &Img)
{
    if (m_lpFileName != nullptr)
    {
        return Img.Save(m_lpFileName) == S_OK;
    }
    return false;
}

LRESULT CozyCaptureWindow::OnRightButtonClicked(UINT uMsg, WPARAM  wParam, LPARAM lParam, BOOL& bHandled)
{
    if (m_ThisStatus == CaptureStatus::S_None)
    {
        Exit();
    }
    else if (m_ThisStatus == CaptureStatus::S_Selected)
    {
        m_IsMoved       = false;
        m_ThisStatus    = CaptureStatus::S_None;
    }
    return 0;
}

LRESULT CozyCaptureWindow::OnPaint(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    HDC hdc = GetWindowDC();
    BlendImage();
    m_ResultImg.Draw(hdc, 0, 0, m_lWidth, m_lHeight);
    ReleaseDC(hdc);
    return 0;
}

LRESULT CozyCaptureWindow::OnClose(UINT   uMsg, WPARAM   wParam, LPARAM   lParam, BOOL&   bHandled)
{
    Exit();
    return 0;
}

void CozyCaptureWindow::BlendImage()
{
    CImageDC resultDc(m_ResultImg);
    CImageDC maskDc(m_MaskImg);

    m_CaptureImg.BitBlt(resultDc, 0, 0, m_lWidth, m_lHeight, 0, 0);
    
    if (m_IsMoved)
    {
        RECT ActRect;
        Point2Rect(m_BeginPoint, m_CurrPoint, &ActRect);

        m_MaskImg.AlphaBlend(resultDc, 0, 0, m_lWidth, m_lHeight, 0, 0, m_lWidth, m_lHeight, 150, AC_SRC_OVER);

        m_CaptureImg.BitBlt(
            resultDc, 
            ActRect.left, 
            ActRect.top, 
            ActRect.right - ActRect.left, 
            ActRect.bottom - ActRect.top, 
            ActRect.left,
            ActRect.top, SRCCOPY);

        ::FrameRect(resultDc, &ActRect, ::CreateSolidBrush(RGB(0x1B, 0xA1, 0xE2)));
    }
}

LRESULT CozyCaptureWindow::OnMouseMoveConvert(UINT   uMsg, WPARAM   wParam, LPARAM   lParam, BOOL&   bHandled)
{
    POINT cp { GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    ::ClientToScreen(m_hWnd, &cp);
    return OnMouseMove(uMsg, wParam, Point2LPARAM(cp), bHandled);
}

LRESULT CozyCaptureWindow::OnLeftButtonUpConvert(UINT   uMsg, WPARAM   wParam, LPARAM   lParam, BOOL&   bHandled)
{
    POINT cp{ GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    ::ClientToScreen(m_hWnd, &cp);
    return OnLeftButtonUp(uMsg, wParam, Point2LPARAM(cp), bHandled);
}

LRESULT CozyCaptureWindow::OnLeftButtonDownConvert(UINT   uMsg, WPARAM   wParam, LPARAM   lParam, BOOL&   bHandled)
{
    POINT cp{ GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    ::ClientToScreen(m_hWnd, &cp);
    return OnLeftButtonDown(uMsg, wParam, Point2LPARAM(cp), bHandled);
}

LRESULT CozyCaptureWindow::OnRightButtonClickedConvert(UINT   uMsg, WPARAM   wParam, LPARAM   lParam, BOOL&   bHandled)
{
    POINT cp{ GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam) };
    ::ClientToScreen(m_hWnd, &cp);
    return OnRightButtonClicked(uMsg, wParam, Point2LPARAM(cp), bHandled);
}

void CozyCaptureWindow::Exit()
{
    ::PostQuitMessage(0);
}

LPARAM CozyCaptureWindow::Point2LPARAM(const POINT &p)
{
    return p.y << 16 | p.x;
}

void CozyCaptureWindow::Point2Rect(const POINT &pa, const POINT &pb, RECT * rect)
{
    if (pa.x > pb.x)
    {
        rect->left      = pb.x;
        rect->right     = pa.x;
    }
    else
    {
        rect->left      = pa.x;
        rect->right     = pb.x;
    }

    if (pa.y > pb.y)
    {
        rect->top       = pb.y;
        rect->bottom    = pa.y;
    }
    else
    {
        rect->top       = pa.y;
        rect->bottom    = pb.y;
    }
}