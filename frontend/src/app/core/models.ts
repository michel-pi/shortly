export interface LoginRequest { email: string; password: string; }
export interface LoginResponse { accessToken?: string | null; refreshToken?: string | null; }
export interface UserResponse { id: number; email?: string | null; name?: string | null; roles?: string[] | null; }
export interface ShortLinkResponse { id: number; targetUrl?: string | null; shortCode?: string | null; isActive: boolean; createdAt?: string | null; }
export interface CreateShortLinkRequest { targetUrl?: string | null; isActive: boolean; expiresAt?: string | null; }
export interface UpdateShortLinkRequest { isActive?: boolean | null; expiresAt?: string | null; }
export interface AccessKeyResponse { id: number; token?: string | null; isActive: boolean; createdAt?: string | null; }
export interface CreateAccessKeyRequest { name?: string | null; isActive: boolean; expiresAt?: string | null; }
export interface UpdateAccessKeyRequest { name?: string | null; isActive?: boolean | null; expiresAt?: string | null; }
export interface ShortLinkEngagementSummaryResponse { totalClicks: number; totalClients: number; }