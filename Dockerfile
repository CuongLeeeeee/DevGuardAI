# =============================================================
# DevGuardAI — Dockerfile (Multi-stage build)
# Đặt file này tại ROOT của solution (cùng cấp với .sln)
# =============================================================

# ── Stage 1: Build ────────────────────────────────────────────
# Dùng 9.0.310 khớp với version máy dev (tránh lỗi ResolvePackageAssets)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy toàn bộ source code trước
COPY . .

# Restore + Publish trong 1 lệnh (bỏ --no-restore để tránh lỗi MSB4018)
RUN dotnet publish DevGuardAI.API/DevGuardAI.API.csproj \
    -c Release \
    -o /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Tạo user không có quyền root (bảo mật)
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Copy artifacts từ stage build
COPY --from=build /app/publish .

# Phân quyền cho appuser
RUN chown -R appuser:appgroup /app
USER appuser

# Expose port HTTP
EXPOSE 8080

# Healthcheck — gọi /health endpoint (hoặc thay bằng endpoint có sẵn)
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "DevGuardAI.API.dll"]