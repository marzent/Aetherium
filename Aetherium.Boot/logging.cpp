#include <fstream>
#include <memory>
#include <vector>
#include <chrono>
#include <iostream>
#include <stdexcept>

#include "logging.hpp"

static bool skipLogFileWrite = false;
static std::fstream logFile;

void logging::start_file_logging(const std::filesystem::path& logPath, bool redirect_stderrout) {
    constexpr long MaxLogFileSize = 1 * 1024 * 1024;
    constexpr long MaxOldFileSize = 10 * 1024 * 1024;

    if (logFile.is_open())
        return;

    const auto oldPath = std::filesystem::path(logPath).replace_extension(".old.log");
    
    logFile.open(logPath, std::ios::out | std::ios::in);
    logFile.seekg(0, std::ios::end);

    // 1. Move excess data from logPath to oldPath
    if (logFile.tellg() > MaxLogFileSize) {
        const auto amountToMove = std::min<long>((long)logFile.tellg() - MaxLogFileSize, MaxOldFileSize);
        logFile.seekg(MaxLogFileSize + amountToMove, std::ios::end);

        std::ofstream oldLogFile(oldPath, std::ios::out | std::ios::app);
        std::string line;
        while (getline(logFile, line))
            oldLogFile << line << '\n';
        
        oldLogFile.close();
    }

    // 2. Cull each of .log and .old files
    for (const auto& [path, maxSize] : std::initializer_list<std::pair<std::filesystem::path, long>>{
        {oldPath, MaxOldFileSize},
        {logPath, MaxLogFileSize},
    }) {
        try {
            std::ifstream inFile(path, std::ios::in | std::ios::binary);
            inFile.seekg(0, std::ios::end);
            if (inFile.tellg() <= MaxLogFileSize)
                continue;
            inFile.seekg(maxSize, std::ios::end);

            std::vector<char> buffer;
            buffer.reserve(maxSize);
            std::copy(std::istreambuf_iterator<char>(inFile),
                      std::istreambuf_iterator<char>(),
                      std::back_inserter(buffer));
            inFile.close();
            
            std::ofstream outFile(path, std::ios::out | std::ios::binary | std::ios::trunc);
            std::copy(buffer.cbegin(),
                      buffer.cend(),
                      std::ostream_iterator<char>(outFile));
            outFile.close();
        } catch (...) {
            // ignore
        }
    }
    
    logFile.close();
    logFile.open(logPath, std::ios::out | std::ios::app);
    
    if (!logFile.good())
        throw std::invalid_argument("bad log file path");
    
    if (redirect_stderrout) {
        std::cerr.rdbuf(logFile.rdbuf());
        std::cout.rdbuf(logFile.rdbuf());
        skipLogFileWrite = true;
    }
}

template<>
void logging::print<char>(Level level, const char* s) {
    const auto now = std::chrono::system_clock::now();
    const std::time_t currentTime = std::chrono::system_clock::to_time_t(now);
    const std::tm* st = std::localtime(&currentTime);
    
    std::string estr;
    switch (level) {
        case Level::Verbose:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/VRB] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        case Level::Debug:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/DBG] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        case Level::Info:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/INF] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        case Level::Warning:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/WRN] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        case Level::Error:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/ERR] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        case Level::Fatal:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/FTL] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
        default:
            estr = fmt::format("[{:02}:{:02}:{:02} CPP/???] {}\n", st->tm_hour, st->tm_min, st->tm_sec, s);
            break;
    }

    std::cerr << estr;

    if (!skipLogFileWrite) {
        logFile << estr;
        logFile.flush();
    }
}
